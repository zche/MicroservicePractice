using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.Api.Helper
{
    public class GlobalObject
    {
        public static string DefaultConfigValue { get; set; } = "undefined";
        public static string Namespace_ServiceDiscovery { get; set; } = "ServiceDiscovery";
        public static string Namespace_DefaultKey { get; set; } = "content";
    }
}
