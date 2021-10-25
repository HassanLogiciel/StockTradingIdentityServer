using IdentityServer4.Models;
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
               new ApiResource("TransactionApi", "Transaction Api for stock trading app normal user"),
               new ApiResource("TransactionAdminApi", "Transaction Api for stock trading app admin user"),
               new ApiResource("NWalletApi", "Wallet Api for stock trading app normal user"),
               new ApiResource("UserApi", "User Api For Admin"),
               new ApiResource("NUserApi", "User Api For Normal User")
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
                    AllowedScopes = { "TransactionApi", "NUserApi", "NWalletApi" },
                    AccessTokenLifetime = 172800

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
                    AllowedScopes = { "UserApi","TransactionAdminApi" },
                    AccessTokenLifetime = 172800
                }
            };
        }

        public static IEnumerable<ApiScope> Scopes()
        {
            return new List<ApiScope>()
                {
                   new ApiScope("TransactionApi", "Transaction Api for stock trading app normal user"),
                   new ApiScope("UserApi", "Admin Api For User Managements"),
                   new ApiScope("NUserApi", "User Api For Normal User"),
                   new ApiScope("TransactionAdminApi", "Transaction Api for stock trading app admin user"),
                   new ApiScope("NWalletApi", "Wallet Api for stock trading app normal user")
                };
        }
    }
}
