using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using User.Identity.Configuration;

namespace User.Identity.Controllers
{
    [Produces("application/json")]
    [Route("api/Default")]
    public class DefaultController : Controller
    {
        private readonly IOptionsMonitor<ServiceDiscoveryOptions> _serviceDiscoveryOptions;
        private readonly IConfiguration _configuration;
        public DefaultController(IOptionsMonitor<ServiceDiscoveryOptions> serviceDiscoveryOptions, IConfiguration configuration)
        {
            _serviceDiscoveryOptions = serviceDiscoveryOptions;
            _configuration = configuration;
        }
        [HttpGet]
        public string Get()
        {
            //Console.WriteLine(_configuration["test"]);
            //return _configuration["content"];
            return GlobalObj.ServiceDiscovery.UserServiceName;
        }
    }
}