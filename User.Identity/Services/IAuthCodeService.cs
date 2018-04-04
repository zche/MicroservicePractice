using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public interface IAuthCodeService
    {
        /// <summary>
        /// 自定义手机验证码模式
        /// </summary>
        /// <param name="tel">手机号</param>
        /// <param name="authCode">验证码标识</param>
        /// <returns></returns>
        bool Validate(string tel,string authCode);
    }
}
