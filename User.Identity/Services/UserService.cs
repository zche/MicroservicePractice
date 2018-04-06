using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using User.Identity.Configuration;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private IHttpClient _httpClient;
        private string _uesrServiceUrl;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClient httpClient, IOptions<ServiceDiscoveryOptions> options, IDnsQuery dnsQuery, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            var address = dnsQuery.ResolveService("service.consul", options.Value.UserServiceName);
            var host = address.FirstOrDefault()?.HostName;
            var port = address.FirstOrDefault().Port;
            _uesrServiceUrl = $"http://{host}:{port}";
            _logger = logger;
        }
        public async Task<int> CheckOrCreate(string tel)
        {
            var content = new Dictionary<string, string> { { "tel", tel } };
            var formContent = new FormUrlEncodedContent(content);
            try
            {
                var response = await _httpClient.PostAsync(_uesrServiceUrl + "/api/users/check-or-create", content);
                if (response.IsSuccessStatusCode)
                {
                    var userId = await response.Content.ReadAsStringAsync();
                    int.TryParse(userId, out int intUserId);
                    return intUserId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CheckOrCreate失败，异常信息为{ex.ToString()}");
                throw;
            }
            return 0;
        }
    }
}
