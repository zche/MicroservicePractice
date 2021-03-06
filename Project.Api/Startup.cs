﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure;
using System.Reflection;
using Project.Api.Application.Services;
using Project.Api.Application.Queries;
using Project.Api.Configurations;
using System.IdentityModel.Tokens.Jwt;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Project.Api.Helper;
using Project.Domain.AggregatesModel;
using Project.Infrastructure.Repositories;
using Newtonsoft.Json;
using DotNetCore.CAP;
using IdentityServer4.Configuration;
using DotNetCore.CAP.Dashboard.NodeDiscovery;

namespace Project.Api
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
            services.AddDbContext<ProjectContext>(opt =>
            {
                opt.UseMySQL(Configuration.GetValue("MysqlProject",GlobalObject.DefaultConfigValue),
                    builder => builder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name));
            });
            services.AddScoped<IRecommend, RecommendService>()
                    .AddScoped<IProjectQueries, ProjectQueriesService>(sp=> {
                        return new ProjectQueriesService(Configuration.GetValue("MysqlProject", GlobalObject.DefaultConfigValue));
                    });
            Assembly[] assemblies = {
                Assembly.Load(new AssemblyName("Project.Api")),
                Assembly.Load(new AssemblyName("Project.Domain")),
                Assembly.Load(new AssemblyName("Project.Infrastructure"))
                    };
            services.AddMediatR(assemblies);

            //services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
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
                    opt.Audience = "project_api";
                    opt.Authority = "http://localhost";
                });
            services.AddScoped<IProjectRepository, ProjectRepository>();
            var capDiscoveryConfig = Configuration.GetSection(GlobalObject.Namespace_CAPDiscovery);
            var strCAPDiscoveryConfig = capDiscoveryConfig.GetValue(GlobalObject.Namespace_DefaultKey, GlobalObject.DefaultConfigValue);
            DotNetCore.CAP.Dashboard.NodeDiscovery.DiscoveryOptions objCAPDiscovery = JsonConvert.DeserializeObject<DotNetCore.CAP.Dashboard.NodeDiscovery.DiscoveryOptions>(strCAPDiscoveryConfig);
            services.AddCap(opt =>
            {
                opt.UseEntityFramework<ProjectContext>().UseRabbitMQ("localhost").UseDashboard();
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
            services.AddMvc(opt => opt.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalObject.App = app;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCapDashboard();
            app.UseMvc();
        }
    }
}
