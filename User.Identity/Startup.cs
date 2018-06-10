using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience.Http;
using User.Identity.Configuration;
using User.Identity.Services;
using User.Identity.Infrastructure;
using Microsoft.AspNetCore.Http;
using User.Identity.Authentication;
using User.Identity.Helper;
using Newtonsoft.Json;
using zipkin4net.Middleware;
using Microsoft.Extensions.Hosting;

namespace User.Identity
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
            services.AddIdentityServer()
                .AddProfileService<ProfileService>()
                .AddExtensionGrantValidator<Authentication.SmsAuthCodeValidator>()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(InMemoryConfiguration.Clients())
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryConfiguration.ApiResources());

            services.AddScoped<IAuthCodeService, TestAuthCodeService>();
            services.AddScoped<IUserService,UserService>();

            services.AddSingleton<IHostedService, HostedService>();

            //services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));

            var serviceDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_ServiceDiscovery);
            var strServiceDiscoveryConfig = serviceDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            ServiceDiscoveryOptions objServiceDiscovery = JsonConvert.DeserializeObject<ServiceDiscoveryOptions>(strServiceDiscoveryConfig);

            services.Configure<ServiceDiscoveryOptions>(opt =>
            {
                opt.Consul = objServiceDiscovery.Consul;
                opt.UserServiceName = objServiceDiscovery.UserServiceName;
            });

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            //services.AddSingleton(new HttpClient());
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(typeof(ResilienceHttpClientFactory),sp=> {
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var retryCount = 5;
                var exceptionsAllowedBeforeBreaking = 5;
                return new ResilienceHttpClientFactory(logger,httpContextAccessor,retryCount,exceptionsAllowedBeforeBreaking);
            });
            services.AddSingleton<IHttpClient>(sp=>sp.GetRequiredService<ResilienceHttpClientFactory>().GetResilientHttpClient());

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            GlobalObject.App = app;
            app.UseTracing("identity_api");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseMvc();
            
        }
    }
}
