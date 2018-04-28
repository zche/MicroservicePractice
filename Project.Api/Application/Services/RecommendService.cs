using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Api.Application.Services
{
    public class RecommendService : IRecommend 
    {
        public Task<bool> IsRecommendedProject(int projectId, int userId)
        {
            return Task.FromResult(true);
        }
    }
}
