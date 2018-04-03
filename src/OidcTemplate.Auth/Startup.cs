using System.IO;
using System.Reflection;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSoftware.OidcTemplate.Auth.Certificates;
using OpenSoftware.OidcTemplate.Auth.Configuration;
using OpenSoftware.OidcTemplate.Auth.DatabaseSeed;
using OpenSoftware.OidcTemplate.Auth.Services;
using OpenSoftware.OidcTemplate.Data;
using OpenSoftware.OidcTemplate.Domain.Configuration;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Auth
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly int _sslPort = 443;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;

            TelemetryConfiguration.Active.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                var launchConfiguration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile(Path.Combine("Properties", "launchSettings.json"))
                    .Build();
                // During development we won't be using port 443
                _sslPort = launchConfiguration.GetValue<int>("iisSettings::iisExpress:sslPort");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var domainSettings = new DomainSettings();
            Configuration.GetSection(nameof(DomainSettings)).Bind(domainSettings);
            services.Configure<DomainSettings>(options => Configuration.GetSection(nameof(DomainSettings)).Bind(options));

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
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                //options.Filters.Add(new RequireHttpsAttribute());
                options.SslPort = _sslPort;
            });

            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/Account/login";
                    options.UserInteraction.LogoutUrl = "/Account/logout";
                })
                // Replace with your certificate's thumbPrint, path, and password
                .AddSigningCredential(
                    CertificateLoader.Load(
                        "701480955FFC6E5423A267A37F5968E28E4FF31B",
                        Path.Combine(_env.ContentRootPath, "Certificates", "example.pfx"),
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
                    //options.Filters.Add(new RequireHttpsAttribute());
                    options.SslPort = _sslPort;
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
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
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
