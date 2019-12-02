using Microsoft.IdentityModel.Tokens;

namespace Djelato.Services.JWT.Interfaces
{
    public interface IJwtSigningDecodingKey
    {
        SecurityKey GetKey();
    }
}
