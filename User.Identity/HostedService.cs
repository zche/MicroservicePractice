using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using User.Identity.Helper;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

namespace User.Identity
{
    public class HostedService : IHostedService
    {
        public HostedService()
        {
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterZipkinTrace();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            TraceManager.Stop();
            return Task.CompletedTask;
        }

        private void RegisterZipkinTrace()
        {
            TraceManager.SamplingRate = 1.0f;
            var loggerFactory = GlobalObject.App.ApplicationServices.GetService<ILoggerFactory>();
            var logger = new TracingLogger(loggerFactory, "zipkin4net");
            var httpSender = new HttpZipkinSender("http://47.97.126.205:9411","application/json");
            var tracer = new ZipkinTracer(httpSender,new JSONSpanSerializer(),new Statistics());
            var consoleTracer = new ConsoleTracer();
            TraceManager.RegisterTracer(tracer);
            TraceManager.RegisterTracer(consoleTracer);
            TraceManager.Start(logger);
        }
    }
}
