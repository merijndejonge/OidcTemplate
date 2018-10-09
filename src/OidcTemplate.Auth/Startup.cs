using System.IO;
using System.Reflection;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSoftware.OidcTemplate.Auth.Certificates;
using OpenSoftware.OidcTemplate.Auth.Configuration;
using OpenSoftware.OidcTemplate.Auth.DatabaseSeed;
using OpenSoftware.OidcTemplate.Data;
using OpenSoftware.OidcTemplate.Domain.Configuration;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Auth
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var domainSettings = new DomainSettings();
            var section = Configuration.GetSection(nameof(DomainSettings));
            section.Bind(domainSettings);
            services.Configure<DomainSettings>(options => section.Bind(options));

            var appSettings = new AppSettings();
            Configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

            var connectionString = appSettings.ConnectionStrings.AuthContext;
            var migrationsAssembly = typeof(DataModule).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<IdentityContext>(o => o.UseSqlServer(connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(migrationsAssembly)));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/Identity/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
                })
                // Replace with your certificate's thumbPrint, path, and password
                .AddSigningCredential(
                    CertificateLoader.Load(
                        "701480955FFC6E5423A267A37F5968E28E4FF31B",
                        Path.Combine(_hostingEnvironment.ContentRootPath, "Certificates", "example.pfx"),
                        "OidcTemplate",
                        false))
                .AddInMemoryApiResources(Domain.Authentication.Resources.GetApis(domainSettings.Api))
                .AddInMemoryIdentityResources(Domain.Authentication.Resources.GetIdentityResources())
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds
                })
                .AddAspNetIdentity<ApplicationUser>()
                ;

            services.AddMvc(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IClientStore, ClientStore>();
            services.AddScoped<ISeedAuthService, SeedAuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
