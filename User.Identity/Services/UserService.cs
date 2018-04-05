using DnsClient;
using Microsoft.Extensions.Options;
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
        private HttpClient _httpClient;
        private string _uesrServiceUrl;

        public UserService(HttpClient httpClient, IOptions<ServiceDiscoveryOptions> options, IDnsQuery dnsQuery)
        {
            _httpClient = httpClient;
            var address = dnsQuery.ResolveService("service.consul", options.Value.UserServiceName);
            var host = address.FirstOrDefault()?.HostName;
            var port = address.FirstOrDefault().Port;
            _uesrServiceUrl = $"http://{host}:{port}";
        }
        public async Task<int> CheckOrCreate(string tel)
        {
            var content = new Dictionary<string, string> { { "tel", tel } };
            var formContent = new FormUrlEncodedContent(content);
            var response = await _httpClient.PostAsync(_uesrServiceUrl + "/api/users/check-or-create", formContent);
            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int intUserId);
                return intUserId;
            }
            return 0;
        }
    }
}
