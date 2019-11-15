using AutoMapper;
using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoManager _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IHasher _hasher;

        public UserService(IMongoManager userManager, 
            ILogger<UserService> logger, 
            IMapper mapper, 
            IHasher hasher)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _hasher = hasher;
        }

        public async Task<ServiceResult> AddAsync(UserModel model)
        {
            User user = _mapper.Map<User>(model);

            user.Salt = _hasher.GetSalt();
            bool isEmpty = user.Salt.All(x => x == default(byte));
            if (isEmpty)
            {
                _logger.LogError("Function add didn't fill in the salt");
                _logger.LogTrace("Djelato.Services.Services.UserService.Add()");

                var errorResult = new ServiceResult(false, "User didn't save");
                return errorResult;
            }

            user.PasswordHash = _hasher.HashPassword(model.Password, user.Salt);
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogError("Function add didn't hash the password in the salt");
                _logger.LogTrace("Djelato.Services.Services.UserService.Add()");

                var errorResult = new ServiceResult(false, "User didn't save");
                return errorResult;
            }

            await _userManager.UserManager.AddAsync(user);

            bool isSaved = await _userManager.UserManager.CheckAsync(model.Email);
            if (!isSaved)
            {
                _logger.LogError($"User - {model.Name} - didn't save in database");
                _logger.LogTrace("Djelato.Services.Services.UserService.Add()");

                var errorResult = new ServiceResult(false, "User didn't save");
                return errorResult;
            }

            var result = new ServiceResult(true, null);
            return result;
        }

        public async Task<bool> ConfirmEmailAsync(string email)
        {
            bool isExist = await _userManager.UserManager.CheckAsync(email);
            if (!isExist)
            {
                _logger.LogError("User can't confirm email because user with this email doesn't exist in database");
                _logger.LogTrace("Djelato.Services.Services.UserService.CheckByEmailAsync()");
                return false;
            }

            var user = await _userManager.UserManager.GetAsync(email);
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;

                var replaceResult = await _userManager.UserManager.ReplaceOneAsync(user.Id, user);
                if (!replaceResult.IsAcknowledged)
                {
                    _logger.LogError("User can't confirm email because the field in database didn't update to true");
                    _logger.LogTrace("Djelato.Services.Services.UserService.CheckByEmailAsync()");
                    return false;
                }
            }            

            return true;
        }

        public async Task<bool> CheckByEmailAsync(string email)
        {
            bool isExist = await _userManager.UserManager.CheckAsync(email);
            if (isExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
