using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Project.Api.Configurations;
using Project.Api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Project.Api
{
    public class HostedService : IHostedService
    {
        private readonly IConsulClient _consul;
        private readonly ServiceDiscoveryOptions _serviceOption;
        private List<string> lstServiceIds = new List<string>();
        public HostedService(IOptions<ServiceDiscoveryOptions> serviceOption, IConsulClient consul)
        {
            _serviceOption = serviceOption.Value;
            _consul = consul;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var features = GlobalObject.App.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));
            foreach (var address in addresses)
            {
                var serviceId = $"{_serviceOption.ServiceName}_{address.Host}:{address.Port}";
                lstServiceIds.Add(serviceId);
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = _serviceOption.ServiceName,
                    Port = address.Port
                };

                _consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var serviceId in lstServiceIds)
            {
                _consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
            return Task.CompletedTask;
        }
    }
}
