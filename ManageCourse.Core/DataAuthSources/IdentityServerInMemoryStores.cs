using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DataAuthSources
{
    public class IdentityServerInMemoryStores
    {
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("courseapi.read")
        };
        public static IEnumerable<ApiResource> ApiResources
        {
            get
            {
                return new[]
                {
                    new ApiResource("courseapi")
                    {
                        Scopes = new List<string> { "courseapi.read" },
                        ApiSecrets = new List<Secret> {new Secret("CourseApi".Sha256())},
                        UserClaims = new List<string> {"role"}
                    }
                };
            }
        }
        public static IEnumerable<Client> Clients => new[]
        {
            new Client
            {
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowOfflineAccess = true,
                AllowedScopes = { "courseapi.read" },
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientId = "courseclient",
                ClientSecrets = { new Secret("CourseApi".Sha256()) },
                UpdateAccessTokenClaimsOnRefresh = true,
            },
            new Client
            {
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowOfflineAccess = true,
                AllowedScopes = { "courseapi.read" },
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientId = "courseclientexternallogin",
                RedirectUris = {$"{ConfigConstant.URL_CLIENT}/login" },
                ClientSecrets = { new Secret("CourseApi".Sha256()) },
                UpdateAccessTokenClaimsOnRefresh = true,
            }
        };
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                return new List<IdentityResource>
                {
                    new IdentityResource(IdentityServerConstants.StandardScopes.OpenId, "Your user identifier", new[] { JwtClaimTypes.Subject })
                    {
                        Required = true,
                    },
                    new IdentityResource(IdentityServerConstants.StandardScopes.Profile, "User profile", new[]
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.FamilyName,
                        JwtClaimTypes.GivenName,
                        JwtClaimTypes.MiddleName,
                        JwtClaimTypes.NickName,
                        JwtClaimTypes.PreferredUserName,
                        JwtClaimTypes.Profile,
                        JwtClaimTypes.Picture,
                        JwtClaimTypes.WebSite,
                        JwtClaimTypes.Gender,
                        JwtClaimTypes.BirthDate,
                        JwtClaimTypes.ZoneInfo,
                        JwtClaimTypes.Locale,
                        JwtClaimTypes.UpdatedAt
                    })
                    {
                        Emphasize = true,
                        Required = true,
                    },
                };
            }
        }
    }
}
