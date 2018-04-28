using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Api.Application.Services
{
    public interface IRecommend
    {
        Task<bool> IsRecommendedProject(int projectId, int userId);
    }
}
