﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Identity.Dtos;

namespace User.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机号是否注册，如果没有注册就立即注册一个
        /// </summary>
        /// <param name="tel"></param>
        Task<UserInfo> CheckOrCreate(string tel);
    }
}
