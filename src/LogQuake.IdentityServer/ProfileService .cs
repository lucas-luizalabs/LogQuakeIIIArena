using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.IdentityServer
{
    /// <summary>
    /// Classe com a finalidade de inserir as Claims no objeto Client quando o Grant for GrantTypes.ClientCredentials.
    /// por padrão para esse tipo de autenticação não envia a API protegida não recebe as Claims
    /// </summary>
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims.AddRange(context.Subject.Claims);

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}
