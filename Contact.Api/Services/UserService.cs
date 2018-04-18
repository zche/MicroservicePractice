using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.Api.Configurations;
using Contact.Api.Dtos;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience.Http;

namespace Contact.Api.Services
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

        public async Task<BaseUserInfo> GetBaseUserInfoAsync(int userId)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(_uesrServiceUrl + $"/api/users/userInfo/{userId}");
                return JsonConvert.DeserializeObject<BaseUserInfo>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBaseUserInfo失败，异常信息为{ex.ToString()}");
                throw;
            }
        }
    }
}
