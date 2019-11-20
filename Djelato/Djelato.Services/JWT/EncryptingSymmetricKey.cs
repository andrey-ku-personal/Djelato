using Djelato.Services.JWT.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Djelato.Services.JWT
{
    public class EncryptingSymmetricKey : IJwtEncryptingEncodingKey, IJwtEncryptingDecodingKey
    {
        private readonly SymmetricSecurityKey _secretKey;

        public string SigningAlgorithm { get; } = JwtConstants.DirectKeyUseAlg;

        /// <summary>
        /// JWT Supports:
        /// A128CBC-HS256 - AES_128_CBC_HMAC_SHA_256 authenticated encryption
        /// A192CBC-HS384 - AES_192_CBC_HMAC_SHA_384 authenticated encryption
        /// A256CBC-HS512 - AES_256_CBC_HMAC_SHA_512 authenticated encryption
        /// A128GCM - AES GCM using 128-bit key
        /// A192GCM - AES GCM using 192-bit key
        /// A256GCM - AES GCM using 256-bit key
        /// </summary>
        public string EncryptingAlgorithm { get; } = SecurityAlgorithms.Aes256CbcHmacSha512;

        public EncryptingSymmetricKey(string key)
        {
            this._secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public SecurityKey GetKey() => this._secretKey;
    }
}
