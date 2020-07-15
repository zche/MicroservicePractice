using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using User.Identity.Configuration;
using User.Identity.Helper;

namespace User.Identity
{
    class ConfigurationMonitor
    {
        private IConfig config;
        private IConfig anotherConfig;
        private IConfiguration _configuration { get;}

        public ConfigurationMonitor(IConfiguration configuration)
        {
            _configuration = configuration;
            var configurationManager = new ApolloConfigurationManager();
            config = configurationManager.GetAppConfig().Result;
            anotherConfig = configurationManager.GetConfig(GlobalObject.Namespace_ServiceDiscovery + ".json").Result;
            config.ConfigChanged += OnChanged;
            anotherConfig.ConfigChanged += OnChanged;
        }

        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            ServiceDiscoveryOptions objServiceDiscovery = JsonConvert.DeserializeObject<ServiceDiscoveryOptions>(_configuration["ServiceDiscovery:content"]);
            GlobalObj.ServiceDiscovery = objServiceDiscovery;
            Console.WriteLine("Changes for namespace {0}", changeEvent.Config);
            foreach (var change in changeEvent.Changes)
            {
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    change.Value.PropertyName, change.Value.OldValue, change.Value.NewValue, change.Value.ChangeType);
            }
        }
    }
}