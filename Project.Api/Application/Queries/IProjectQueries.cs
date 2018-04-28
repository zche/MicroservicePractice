using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Api.Application.Queries
{
    public interface IProjectQueries
    {
        Task<dynamic> GetProjectsByUserIdAsync(int projectId);
        Task<dynamic> GetProjectDetail(int prjectId);
    }
}
