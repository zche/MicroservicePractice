using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Data
{
    public class UserContextSeed
    {
        private ILogger<UserContextSeed> _logger;
        public UserContextSeed(ILogger<UserContextSeed> logger)
        {
            _logger = logger;
        }
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int retry = 0)
        {
            var v = retry;
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserContextSeed>>();
                try
                {
                    var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                    logger.LogDebug("Begin UserContextSeed SeedAsync");
                    userContext.Database.Migrate();
                    if (!userContext.Users.Any())
                    {
                        userContext.Users.Add(new Models.AppUser { Name = "check" });
                        userContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (v < 10)
                    {
                        v++;
                        logger.LogError(ex.ToString());
                        await SeedAsync(applicationBuilder, loggerFactory, retry);
                    }
                }
            }
        }
    }
}
