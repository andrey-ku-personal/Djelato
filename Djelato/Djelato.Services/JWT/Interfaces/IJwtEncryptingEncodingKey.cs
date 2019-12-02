using Microsoft.IdentityModel.Tokens;

namespace Djelato.Services.JWT.Interfaces
{
    public interface IJwtEncryptingEncodingKey
    {
        string SigningAlgorithm { get; }

        string EncryptingAlgorithm { get; }

        SecurityKey GetKey();
    }
}
