﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo;
using Contact.Api.Helper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contact.Api
{
    public class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(ConfigureConfiguration)
                .UseStartup<Startup>()
                .Build();

        private static void ConfigureConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder) =>
            builder.AddApollo(builder.Build().GetSection("apollo"))
            .AddDefault()
            .AddNamespace(GlobalObject.Namespace_ServiceDiscovery + ".json", GlobalObject.Namespace_ServiceDiscovery)
            .AddNamespace(GlobalObject.Namespace_MongoSettings + ".json", GlobalObject.Namespace_MongoSettings)
            .AddNamespace(GlobalObject.Namespace_CAPDiscovery + ".json", GlobalObject.Namespace_CAPDiscovery);
    }
}
