using System;
using System.Security.Claims;

namespace Nuuvify.CommonPack.Security.Abstraction
{
    public interface IJwtBuilder
    {
        long ToUnixEpochDate(DateTime date);
        IJwtBuilder WithJwtClaims();

        IJwtBuilder WithJwtUserClaims<T>(PersonWithRolesQueryResult personGroups)
            where T : IPersonBuilder;

        ClaimsIdentity GetClaimsIdentity();

        string BuildToken();

        CredentialToken GetUserToken();
        bool CheckTokenIsValid(string token);

    }
}