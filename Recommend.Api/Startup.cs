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
                opt.UseMySQL(Configuration.GetConnectionString("MysqlRecommend"));
            });

            services.AddScoped<IntegrationEventHandlers.ProjectCreatedIntegrationEventHandler>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
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
            services.AddCap(opt =>
            {
                opt.UseMySql("server=localhost;port=3306;database=beta_recommend;userid=root;password=Zrf123456!;SslMode=none;")
                .UseRabbitMQ("localhost")
                .UseDashboard();
                opt.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5681;
                    d.NodeId = 4;
                    d.NodeName = "CAP RecommendApi节点";
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
            app.UseCap();
            app.UseMvc();
        }
    }
}
