using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Services.JWT.Interfaces
{
    public interface IJwtEncryptingEncodingKey
    {
        string SigningAlgorithm { get; }

        string EncryptingAlgorithm { get; }

        SecurityKey GetKey();
    }
}
