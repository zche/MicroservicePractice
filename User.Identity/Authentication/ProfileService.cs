using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using User.Identity.Services;

namespace User.Identity.Authentication
{
    public class ProfileService : IProfileService
    {
        //Get user profile date in terms of claims when calling /connect/userinfo
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                context.IssuedClaims = context.Subject.Claims.ToList();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        //check if user account is active.
        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }

    }
}
