﻿using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockTrader
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new List<IdentityResource>()
            {
                //Client ClientCredentials
                new IdentityResources.OpenId()
            };
        }
        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
               new ApiResource("tradingAppLoginAPI", "Login Api for stock trading app"),
               new ApiResource("UserApi", "Admin Api For User Managements")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes =
                    {
                        GrantType.ResourceOwnerPassword
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "tradingAppLoginAPI" }
                },
                new Client
                {
                    ClientId = "adminClient",
                    AllowedGrantTypes =
                    {
                        GrantType.ResourceOwnerPassword
                    },
                    ClientSecrets =
                    {
                        new Secret("adminSecret".Sha256())
                    },
                    AllowedScopes = { "UserApi" }    
                }
            };
        }

        public static IEnumerable<ApiScope> Scopes()
        {
            return new List<ApiScope>()
                {
                   new ApiScope("tradingAppLoginAPI", "Login Api for stock trading app"),
                   new ApiScope("UserApi", "Admin Api For User Managements"),
                };
        }
    }
}
