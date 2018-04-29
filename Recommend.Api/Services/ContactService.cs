using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Recommend.Api.Configurations;
using Recommend.Api.Dtos;
using Resilience.Http;

namespace Recommend.Api.Services
{
    public class ContactService : IContactService
    {
        private IHttpClient _httpClient;
        private string _contactServiceUrl;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IHttpClient httpClient, IOptions<ServiceDiscoveryOptions> options, IDnsQuery dnsQuery, ILogger<ContactService> logger)
        {
            _httpClient = httpClient;
            var address = dnsQuery.ResolveService("service.consul", options.Value.ContactServiceName);
            var host = address.FirstOrDefault()?.HostName;
            var port = address.FirstOrDefault().Port;
            _contactServiceUrl = $"http://{host}:{port}";
            _logger = logger;
        }

        public async Task<List<Contact>> GetContactsByUserId(int userId)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(_contactServiceUrl + $"/api/Contact/{userId}");
                return JsonConvert.DeserializeObject<List<Contact>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBaseUserInfo失败，异常信息为{ex.ToString()}");
                throw;
            }
        }
    }
}
