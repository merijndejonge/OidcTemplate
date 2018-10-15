using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSoftware.OidcTemplate.Domain.Authentication;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var domainSettings = new DomainSettings();
            var section = Configuration.GetSection(nameof(DomainSettings));
            section.Bind(domainSettings);
            services.Configure<DomainSettings>(options => section.Bind(options));

            services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddAuthorization()
                .AddJsonFormatters()
                ;
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = domainSettings.Auth.Url;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = domainSettings.Api.Id;
                    options.ApiSecret = domainSettings.Api.Secret;
                });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(domainSettings.Client.Url)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(DomainPolicies.NormalUser,
                    policy => policy.RequireClaim(JwtClaimTypes.Scope, DomainScopes.MvcClientUser));
                options.AddPolicy(DomainPolicies.Admin,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, DomainRoles.Admin));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
