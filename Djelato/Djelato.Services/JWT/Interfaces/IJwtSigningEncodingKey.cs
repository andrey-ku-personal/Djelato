using Microsoft.IdentityModel.Tokens;

namespace Djelato.Services.JWT.Interfaces
{
    public interface IJwtSigningEncodingKey
    {
        string SigningAlgorithm { get; }

        SecurityKey GetKey();
    }
}
