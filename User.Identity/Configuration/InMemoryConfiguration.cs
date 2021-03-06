﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace User.Identity
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("gateway_api", "网关"),
                new ApiResource("contact_api", "通讯录接口"),
                new ApiResource("user_api", "用户接口"),
                new ApiResource("project_api", "项目接口"),
                new ApiResource("recommend_api", "推荐人接口"),
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "mobileId",
                    ClientName = "手机客户端",
                    //AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    AllowOfflineAccess = true,
                    RequireClientSecret=false,
                    AlwaysSendClientClaims=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedGrantTypes =new List<string>{"sms_auth_code" },
                    AllowedScopes = new List<string>
                    {IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "gateway_api",
                        "contact_api",
                        "user_api",
                        "project_api",
                        "recommend_api"
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}