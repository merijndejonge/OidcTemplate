using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using OpenSoftware.OidcTemplate.Domain.Authentication;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Api
{
    public class Startup
    {
        private readonly int _sslPort = 443;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                var launchConfiguration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile(Path.Combine("Properties", "launchSettings.json"))
                    .Build();
                // During development we won't be using port 443
                _sslPort = launchConfiguration.GetValue<int>("iisSettings::iisExpress::sslPort");
            }

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


            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvcCore(options =>
            {
                // options.Conventions.Insert(0, new ModeRouteConvention());
                //                options.Filters.Add(new RequireHttpsAttribute());
                options.SslPort = _sslPort;
            })
                .AddAuthorization()
                .AddJsonFormatters();


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
                options.AddPolicy(DomainPolicies.Admin, policy => policy.RequireClaim(JwtClaimTypes.Role, DomainRoles.Admin));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default");
            app.UseAuthentication();

            app.UseSwaggerUi(typeof(Startup).Assembly, new SwaggerUiSettings());

            app.UseMvc();
        }
    }
}
