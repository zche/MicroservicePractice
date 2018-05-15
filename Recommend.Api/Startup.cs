using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Recommend.Api.Data;
using Recommend.Api.Infrastructure;
using Recommend.Api.Configurations;
using DnsClient;
using Resilience.Http;
using Microsoft.AspNetCore.Http;
using Recommend.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Recommend.Api.Helper;
using Newtonsoft.Json;
using DotNetCore.CAP;

namespace Recommend.Api
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
            services.AddDbContext<RecommendContext>(opt=> {
                opt.UseMySQL(Configuration.GetValue("MysqlRecommend", GlobalObject.DefaultConfigValue));
            });

            services.AddScoped<IntegrationEventHandlers.ProjectCreatedIntegrationEventHandler>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();
            //services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            var serviceDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_ServiceDiscovery);
            var strServiceDiscoveryConfig = serviceDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            ServiceDiscoveryOptions objServiceDiscovery = JsonConvert.DeserializeObject<ServiceDiscoveryOptions>(strServiceDiscoveryConfig);
            services.Configure<ServiceDiscoveryOptions>(opt =>
            {
                opt.Consul = objServiceDiscovery.Consul;
                opt.UserServiceName = objServiceDiscovery.UserServiceName;
                opt.ContactServiceName = objServiceDiscovery.ContactServiceName;
            });

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.Audience = "recommend_api";
                    opt.Authority = "http://localhost";
                    opt.SaveToken = true;
                });

            //services.AddSingleton(new HttpClient());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(typeof(ResilienceHttpClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var retryCount = 5;
                var exceptionsAllowedBeforeBreaking = 5;
                return new ResilienceHttpClientFactory(logger, httpContextAccessor, retryCount, exceptionsAllowedBeforeBreaking);
            });
            services.AddSingleton<IHttpClient>(sp => sp.GetRequiredService<ResilienceHttpClientFactory>().GetResilientHttpClient());
            var capDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_CAPDiscovery);
            var strCAPDiscoveryConfig = capDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            DiscoveryOptions objCAPDiscovery = JsonConvert.DeserializeObject<DiscoveryOptions>(strCAPDiscoveryConfig);
            services.AddCap(opt =>
            {
                opt.UseEntityFramework<RecommendContext>()
                .UseRabbitMQ("localhost")
                .UseDashboard();
                opt.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = objCAPDiscovery.DiscoveryServerHostName;
                    d.DiscoveryServerPort = objCAPDiscovery.DiscoveryServerPort;
                    d.CurrentNodeHostName = objCAPDiscovery.CurrentNodeHostName;
                    d.CurrentNodePort = objCAPDiscovery.CurrentNodePort;
                    d.NodeId = objCAPDiscovery.NodeId;
                    d.NodeName = objCAPDiscovery.NodeName;
                });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCap();
            app.UseMvc();
        }
    }
}
