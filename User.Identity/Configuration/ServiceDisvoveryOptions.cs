﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Configuration
{
    public class ServiceDiscoveryOptions
    {
        public string UserServiceName { get; set; }

        public ConsulOptions Consul { get; set; }
    }
    public class Value
    {
        public string Test { get; set; }
    }
}
