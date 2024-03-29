using System.Threading;
using System.Threading.Tasks;
using Nuuvify.CommonPack.Security.Abstraction;

namespace Nuuvify.CommonPack.Security.JwtCredentials.Interfaces
{
    public interface IJwtStore
    {
        void Set(string username, CredentialToken tokenResult);
        Task<CredentialToken> Get(string username, CancellationToken cancellationToken = default);
        Task Clear(string username, CancellationToken cancellationToken = default);
        Task ClearAll(CancellationToken cancellationToken = default);
    }

}