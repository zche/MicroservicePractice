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
using User.Api.Data;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using User.Api.Configuration;
using Consul;
using Microsoft.Extensions.Hosting;
using User.Api.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using DotNetCore.CAP;
using zipkin4net.Middleware;

namespace User.Api
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
            string connStr = Configuration.GetValue("MysqlUser", GlobalObject.DefaultConfigValue);
            services.AddDbContext<UserContext>(opt =>
            {
                opt.UseMySQL(connStr);
            });
            var serviceDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_ServiceDiscovery);
            var strServiceDiscoveryConfig = serviceDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            ServiceDiscoveryOptions objServiceDiscovery = JsonConvert.DeserializeObject<ServiceDiscoveryOptions>(strServiceDiscoveryConfig);

            services.Configure<ServiceDiscoveryOptions>(opt =>
            {
                opt.Consul = objServiceDiscovery.Consul;
                opt.ServiceName = objServiceDiscovery.ServiceName;
            });
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


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.Audience = "user_api";
                    opt.Authority = "http://localhost";
                });

            var capDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_CAPDiscovery);
            var strCAPDiscoveryConfig = capDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            DiscoveryOptions objCAPDiscovery = JsonConvert.DeserializeObject<DiscoveryOptions>(strCAPDiscoveryConfig);
            services.AddCap(opt =>
            {
                opt.UseEntityFramework<UserContext>().UseRabbitMQ("localhost").UseDashboard();
                //注册到consul
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            GlobalObject.App = app;
            app.UseTracing("user_api");
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            app.UseExceptionHandler("/api/users/Exception");
            app.UseCap();
            //}          
            app.UseAuthentication();
            app.UseMvc();
            
            loggerFactory.AddNLog();//添加NLog 
            NLog.LogManager.LoadConfiguration("nlog.config");
            //UserContextSeed.SeedAsync(app, loggerFactory).Wait();
            InitUserDb(app);
        }

        public void InitUserDb(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                userContext.Database.Migrate();
                if (!userContext.Users.Any())
                {
                    userContext.Users.Add(new Models.AppUser { Name = "check" });
                    userContext.SaveChanges();
                }
            }
        }
    }
}
