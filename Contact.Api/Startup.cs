using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Contact.Api.Configurations;
using Contact.Api.Data;
using Contact.Api.Helper;
using Contact.Api.Infrastructure;
using Contact.Api.IntegrationEvents;
using Contact.Api.Services;
using DnsClient;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience.Http;

namespace Contact.Api
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
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>();
            services.AddScoped<IContactBookRepository, MongoContactBookRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserProfileChangedEventHandler>();

            //services.Configure<MongoDBSetting>(Configuration.GetSection("MongoSettings"));

            var mongoSettingsConfig = Configuration.GetSection(GlobalObject.Namespace_MongoSettings);
            var strMongoSettingsConfig = mongoSettingsConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            MongoDBSetting objMongoSettings = JsonConvert.DeserializeObject<MongoDBSetting>(strMongoSettingsConfig);
            services.Configure<MongoDBSetting>(opt =>
            {
                opt.DataBase = objMongoSettings.DataBase;
                opt.UserName = objMongoSettings.UserName;
                opt.Password = objMongoSettings.Password;
                opt.Services = objMongoSettings.Services;
            });

            services.AddSingleton<ContactContext>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.Audience = "contact_api";
                    opt.Authority = "http://localhost";
                    opt.SaveToken = true;
                });

            //services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            var serviceDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_ServiceDiscovery);
            var strServiceDiscoveryConfig = serviceDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            ServiceDiscoveryOptions objServiceDiscovery = JsonConvert.DeserializeObject<ServiceDiscoveryOptions>(strServiceDiscoveryConfig);
            services.Configure<ServiceDiscoveryOptions>(opt =>
            {
                opt.Consul = objServiceDiscovery.Consul;
                opt.ServiceName = objServiceDiscovery.ServiceName;
                opt.UserServiceName = objServiceDiscovery.UserServiceName;
            });

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
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

            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));
            services.AddSingleton<IHostedService, HostedService>();

            string connStr = Configuration.GetValue("MysqlContact", GlobalObject.DefaultConfigValue);

            var capDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_CAPDiscovery);
            var strCAPDiscoveryConfig = capDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            DiscoveryOptions objCAPDiscovery = JsonConvert.DeserializeObject<DiscoveryOptions>(strCAPDiscoveryConfig);
            services.AddCap(opt =>
            {
                opt.UseMySql(connStr)
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
            GlobalObject.App = app;
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
