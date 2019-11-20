using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.Services.JWT.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoManager _mongoManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IHasher _hasher;
        private readonly IJwtSigningEncodingKey _signingEncodingKey;
        private readonly IJwtEncryptingEncodingKey _encryptingEncodingKey;

        public AuthService(
            IMongoManager mongoManager, 
            ILogger<AuthService> logger, 
            IHasher hasher, 
            IJwtSigningEncodingKey signingEncodingKey,
            IJwtEncryptingEncodingKey encryptingEncodingKey
            )
        {
            _mongoManager = mongoManager;
            _logger = logger;
            _hasher = hasher;
            _signingEncodingKey = signingEncodingKey;
            _encryptingEncodingKey = encryptingEncodingKey;
        }        

        public async Task<ServiceResult> AuthorizeAsync(AuthModel model)
        {
            User user = await _mongoManager.UserManager.GetAsync(model.Email.ToLower());
            if (user == null)
            {
                _logger.LogWarning("User didn't get token. User data isn't loaded from database because the data doesn't exist ");
                _logger.LogTrace("Djelato.Services.Services.GetTokenAsync(AuthModel model)");

                return new ServiceResult(false, "Profile doesn't exist");
            }

            string passHash = _hasher.HashPassword(model.Password, user.Salt);

            if (user.Email != model.Email.ToLower() || user.PasswordHash != passHash)
            {
                return new ServiceResult(false, "Check email and password please");
            }
            else
            {
                return new ServiceResult(true, "User authorized");
            }
        }

        public async Task<bool> CheckUserAsync(string email)
        {
            bool isExist = await _mongoManager.UserManager.CheckAsync(email);
            if (isExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ServiceResult<string>> GetTokenAsync(string email)
        {
            User user = await _mongoManager.UserManager.GetAsync(email.ToLower());
            if (user == null)
            {
                _logger.LogWarning("User didn't get token. User data isn't loaded from database because the data doesn't exist ");
                _logger.LogTrace("Djelato.Services.Services.GetTokenAsync(string email, IJwtSigningEncodingKey signingEncodingKey)");

                return new ServiceResult<string>(false, null, "Profile doesn't exist");
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: "DjelatoApp",
                audience: "DjelatoClient",
                subject: new ClaimsIdentity(claims),
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(24),
                issuedAt: DateTime.Now,
                signingCredentials: new SigningCredentials(
                    _signingEncodingKey.GetKey(),
                    _signingEncodingKey.SigningAlgorithm),
                encryptingCredentials: new EncryptingCredentials(
                    _encryptingEncodingKey.GetKey(),
                    _encryptingEncodingKey.SigningAlgorithm,
                    _encryptingEncodingKey.EncryptingAlgorithm)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogError("JwtSecurityToken class didn't generate token");
                _logger.LogTrace("Djelato.Services.Services.GetTokenAsync(string email, IJwtSigningEncodingKey signingEncodingKey)");

                return new ServiceResult<string>(true, jwtToken, "Token generated");
            }
            else
            {
                _logger.LogInformation($"token generate successfully");

                return new ServiceResult<string>(true, jwtToken, "Token generated");
            }
        }
    }
}
