using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Resilience.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using System.Net.Http;

namespace Contact.Api.Infrastructure
{
    public class ResilienceHttpClientFactory
    {
        private ILogger<ResilientHttpClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _retryCount;
        private readonly int _exceptionCountBeforeBreaking;
        public ResilienceHttpClientFactory(ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor
            , int retryCount, int exceptionCountBeforeBreaking)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _retryCount = retryCount;
            _exceptionCountBeforeBreaking = exceptionCountBeforeBreaking;
        }
        public ResilientHttpClient GetResilientHttpClient() => new ResilientHttpClient(origin => CreatePolicy(), _logger, _httpContextAccessor);

        private Policy[] CreatePolicy()
        {
            return new Policy[] {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(_retryCount,
                retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                (exception,timeSpan,retryCount,context)=>{
                    var msg=$"第{retryCount}次重试，时间间隔为{timeSpan.Seconds},policyKey为{context.PolicyKey},异常信息为{exception}";
                    _logger.LogInformation(msg);
                }),
                Policy.Handle<HttpRequestException>().CircuitBreakerAsync(_exceptionCountBeforeBreaking,TimeSpan.FromMinutes(2),
                (exception, duration) =>
                   {
                        // on circuit opened
                        _logger.LogInformation("熔断器打开");
                   },
                   () =>
                   {
                        // on circuit closed
                        _logger.LogInformation("熔断器重置");
                   })
            };
        }
    }
}
