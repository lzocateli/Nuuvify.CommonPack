using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Nuuvify.CommonPack.Security.Abstraction;
using Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;


namespace Nuuvify.CommonPack.Security.JwtStore.Ef
{
    internal class DatabaseJwtStore<TContext> : IJwtStore
        where TContext : DbContext, IJwtCacheContext
    {
        private readonly TContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<DatabaseJwtStore<TContext>> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;



        public DatabaseJwtStore(TContext context,
            ILogger<DatabaseJwtStore<TContext>> logger,
            IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
            _logger = logger;


            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    //new OpenIdTokenConverter()
                },
            };


        }




        public async Task Clear(string username, CancellationToken cancellationToken = default)
        {

            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogInformation("Não foi informado um username para remover token");
            }
            else
            {
                _logger.LogInformation("Removendo token para: {0}", username);
                await _cache.RemoveAsync(username, cancellationToken);
            }


            await Task.CompletedTask;

        }
        public async Task ClearAll(CancellationToken cancellationToken = default)
        {

            _logger.LogInformation("Removendo todos os tokens");
            var tokens = await _context.Tokens.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var item in tokens)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await Task.FromCanceled(cancellationToken);
                }
                await _cache.RemoveAsync(item.Id, cancellationToken);
            }

            await Task.CompletedTask;

        }


        public async Task<CredentialToken> Get(string username, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.FromCanceled(cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            else
            {
                var resultCache = await _cache.GetStringAsync(username, cancellationToken);
                if (resultCache == null)
                {
                    return null;
                }
                else
                {
                    var token = JsonSerializer.Deserialize<CredentialToken>(resultCache, _jsonSerializerOptions);
                    return token;
                }

            }

        }


        public void Set(string username, CredentialToken tokenResult)
        {

            if (!string.IsNullOrWhiteSpace(username) && tokenResult != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(tokenResult.Expires);

                var resultCache = JsonSerializer.Serialize(tokenResult);
                _cache.SetString(username, resultCache, cacheOptions);

            }


        }



    }
}