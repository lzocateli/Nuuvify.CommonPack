using System;
using Nuuvify.CommonPack.Security.JwtCredentials.Jwk;
using Microsoft.IdentityModel.Tokens;

namespace Nuuvify.CommonPack.Security.JwtCredentials
{
    /// <summary>
    ///  See RFC 7518 - JSON Web Algorithms (JWA) 
    /// - Section 7.1. JSON Web Signature and Encryption Algorithms Registry
    /// - Section 3.1.  "alg" (Algorithm) Header Parameter Values for JWS
    /// </summary>
    /// <remarks>
    /// AES - Not working for JsonWebKey
    /// public static readonly Algorithm A128KW = new Algorithm(SecurityAlgorithms.Aes128KW, KeyType.AES);
    /// public static readonly Algorithm A256KW = new Algorithm(SecurityAlgorithms.Aes256KW, KeyType.AES);
    /// Not supported
    /// https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Supported-Algorithms
    /// public static readonly Algorithm A192KW = new Algorithm("A192KW", KeyType.AES); -> Not supported
    /// </remarks>
    public sealed class Algorithm
    {
        public static readonly Algorithm HS256 = new Algorithm(SecurityAlgorithms.HmacSha256, KeyType.HMAC);
        public static readonly Algorithm HS384 = new Algorithm(SecurityAlgorithms.HmacSha384, KeyType.HMAC);
        public static readonly Algorithm HS512 = new Algorithm(SecurityAlgorithms.HmacSha512, KeyType.HMAC);

        public static readonly Algorithm RS256 = new Algorithm(SecurityAlgorithms.RsaSha256, KeyType.RSA);
        public static readonly Algorithm RS384 = new Algorithm(SecurityAlgorithms.RsaSha384, KeyType.RSA);
        public static readonly Algorithm RS512 = new Algorithm(SecurityAlgorithms.RsaSha512, KeyType.RSA);
        public static readonly Algorithm PS256 = new Algorithm(SecurityAlgorithms.RsaSsaPssSha256, KeyType.RSA);
        public static readonly Algorithm PS384 = new Algorithm(SecurityAlgorithms.RsaSsaPssSha384, KeyType.RSA);
        public static readonly Algorithm PS512 = new Algorithm(SecurityAlgorithms.RsaSsaPssSha512, KeyType.RSA);

        public static readonly Algorithm ES256 = new Algorithm(SecurityAlgorithms.EcdsaSha256, KeyType.ECDsa, JsonWebKeyECTypes.P256);
        public static readonly Algorithm ES384 = new Algorithm(SecurityAlgorithms.EcdsaSha384, KeyType.ECDsa, JsonWebKeyECTypes.P384);
        public static readonly Algorithm ES512 = new Algorithm(SecurityAlgorithms.EcdsaSha512, KeyType.ECDsa, JsonWebKeyECTypes.P521);


        public KeyType KeyType { get; }
        public string Curve { get; }
        public string Selected { get; }

        private Algorithm(string value, KeyType keyType, string curve)
        {
            Selected = value;
            KeyType = keyType;
            Curve = curve;
        }

        private Algorithm(string value, KeyType keyType)
        {
            Selected = value;
            KeyType = keyType;
        }

        public static Algorithm Create(string value, KeyType key)
        {
            return new Algorithm(value, key);
        }

        /// <summary>
        /// See RFC 7518 - JSON Web Algorithms (JWA) - Section 6.1. "kty" (Key Type) Parameter Values
        /// </summary>
        public string Kty()
        {
            return KeyType switch
            {
                KeyType.RSA => JsonWebAlgorithmsKeyTypes.RSA,
                KeyType.ECDsa => JsonWebAlgorithmsKeyTypes.EllipticCurve,
                KeyType.HMAC => JsonWebAlgorithmsKeyTypes.Octet,
                KeyType.AES => JsonWebAlgorithmsKeyTypes.Octet,
                _ => throw new ArgumentOutOfRangeException(typeof(KeyType).Name)
            };
        }

        public static implicit operator string(Algorithm value) => value.Selected;

    }
}