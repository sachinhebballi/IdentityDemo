using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityDemo
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
            services.AddControllers();

            var configurationOptions = new ConfigurationStoreOptions
            {
                ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer("Data Source=.;Initial Catalog=Identity;Integrated Security=true;");
                }
            };

            var configurationBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>().UseSqlServer("Data Source=.;Initial Catalog=Identity;Integrated Security=true;");

            services.AddSingleton(configurationOptions);
            services.AddSingleton(configurationBuilder.Options);


            services.AddIdentityServer(options =>
            {
                options.IssuerUri = "https://localhost:5001/identityserver";
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Discovery = new DiscoveryOptions
                {
                    ShowClaims = true,
                    ShowEndpoints = true,
                    ShowGrantTypes = true,
                    ShowIdentityScopes = true,
                    ShowKeySet = true,
                    ShowResponseModes = true,
                    ShowResponseTypes = true,
                    ShowTokenEndpointAuthenticationMethods = true,
                };
                options.Csp = new CspOptions
                {
                    AddDeprecatedHeader = options.Csp.AddDeprecatedHeader,
                    Level = options.Csp.Level
                };
                options.Authentication = new AuthenticationOptions
                {

                };
                options.UserInteraction = new UserInteractionOptions
                {

                };
                options.Caching = new CachingOptions
                {

                };
                options.Cors = new CorsOptions
                {

                };
                options.Endpoints = new EndpointsOptions
                {

                };
                options.InputLengthRestrictions = new InputLengthRestrictions
                {

                };
                options.AccessTokenJwtType = "JWT";
            })
            .AddDeveloperSigningCredential()
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer("Data Source=.;Initial Catalog=Identity;Integrated Security=true;");
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = (int)TimeSpan.FromMinutes(5).TotalSeconds;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer("Data Source=.;Initial Catalog=Identity;Integrated Security=true;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
