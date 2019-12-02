using Microsoft.IdentityModel.Tokens;

namespace Djelato.Services.JWT.Interfaces
{
    public interface IJwtEncryptingDecodingKey
    {
        SecurityKey GetKey();
    }
}
