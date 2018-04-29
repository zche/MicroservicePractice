using Recommend.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.Api.Services
{
    public interface IUserService
    {
        Task<BaseUserInfo> GetBaseUserInfoAsync(int userId);
    }
}
