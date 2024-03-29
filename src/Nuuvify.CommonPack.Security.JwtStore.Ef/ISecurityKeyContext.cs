using Nuuvify.CommonPack.Security.JwtCredentials.Model;
using Microsoft.EntityFrameworkCore;

namespace Nuuvify.CommonPack.Security.JwtStore.Ef
{
    public interface ISecurityKeyContext
    {
        /// <summary>
        /// A collection of <see cref="T:Nuuvify.CommonPack.Security.JwtCredentials.Model.SecurityKeyWithPrivate" />
        /// </summary>
        DbSet<SecurityKeyWithPrivate> SecurityKeys { get; set; }
    }
}
