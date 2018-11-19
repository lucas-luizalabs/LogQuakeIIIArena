using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace LogQuake.IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource {
                    Name = "role",
                    Description = "Log Quake III Arena",
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("LogQuake", "Log Quake III Arena", new List<string> {"email", "role"})
                // exemplo de criação de de APIResource
                //new ApiResource("dataEventRecords")
                //{
                //    ApiSecrets =
                //    {
                //        new Secret("dataEventRecordsSecret".Sha256())
                //    },
                //    Scopes =
                //    {
                //        new Scope
                //        {
                //            Name = "dataeventrecords",
                //            DisplayName = "Scope for the dataEventRecords ApiResource"
                //        }
                //    },
                //    UserClaims = { "role", "admin", "user", "dataEventRecords", "dataEventRecords.admin", "dataEventRecords.user" }
                //}
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client1", // simulando um aplicativo que tenha somene permissão para consulta de partidas
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    Claims = new[]
                    {
                        new Claim(JwtClaimTypes.Role, "consulta"),
                        new Claim(JwtClaimTypes.Email, "client1@gmail.com")
                    },
                    AllowedScopes = { "LogQuake" }
                },
                new Client
                {
                    ClientId = "client2", // simulando um aplicativo que tenha permissão para upload de arquivo e consulta de partidas
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret2".Sha256())
                    },
                    Claims = new[]
                    {
                        new Claim(JwtClaimTypes.Role, "admin"),
                        new Claim(JwtClaimTypes.Role, "consulta"),
                        new Claim(JwtClaimTypes.Email, "client2@gmail.com")
                    },
                    ClientClaimsPrefix = "",
                    AllowedScopes = { "LogQuake" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "rop.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "LogQuake" }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "isgalamido",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "isgalamido@gmail.com"),
                        new Claim(JwtClaimTypes.Role, "admin"),
                        new Claim(JwtClaimTypes.Role, "consulta")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "dono da bola",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "donodabola@gmail.com"),
                        new Claim(JwtClaimTypes.Role, "consulta")
                    }
                }
            };
        }
    }
}
