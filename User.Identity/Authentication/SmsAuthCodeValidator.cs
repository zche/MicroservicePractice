using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using User.Identity.Services;

namespace User.Identity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        private readonly IAuthCodeService _authCodeService;
        private readonly IUserService _userService;

        public SmsAuthCodeValidator(IAuthCodeService authCodeService, IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;
        }
        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var tel = context.Request.Raw["tel"];
            var code = context.Request.Raw["auth_code"];
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            if (string.IsNullOrEmpty(tel) || string.IsNullOrEmpty(code))
            {
                context.Result = errorValidationResult;
            }
            if (!_authCodeService.Validate(tel, code))
            {
                context.Result = errorValidationResult;
            }
            var userInfo = await _userService.CheckOrCreate(tel);
            if (userInfo == null)
            {
                context.Result = errorValidationResult;
            }
            var claims = new Claim[] {
                new Claim("name",userInfo.Name??string.Empty),
                new Claim("company",userInfo.Company??string.Empty),
                new Claim("title",userInfo.Title??string.Empty),
                new Claim("avatar",userInfo.Avatar??string.Empty),
            };
            var grantResult= new GrantValidationResult(userInfo.Id.ToString(), GrantType, claims);
            
            context.Result = grantResult;
        }
    }
}
