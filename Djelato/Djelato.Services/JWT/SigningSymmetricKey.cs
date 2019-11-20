using Djelato.Services.JWT.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Services.JWT
{
    public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
    {
        private readonly SymmetricSecurityKey _secretKey;

        /// <summary>
        /// Most common alhoritms are:
        /// HMAC + SHA256
        /// RSASSA-PKCS1-v1_5 + SHA256
        /// ECDSA + P-256 + SHA256
        /// </summary>
        public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

        public SigningSymmetricKey(string key)
        {
            this._secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public SecurityKey GetKey() => this._secretKey;
    }
}
