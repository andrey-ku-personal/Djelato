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
        private readonly IMongoRepoManager _manager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IHasher _hasher;

        public UserService(IMongoRepoManager manager, ILogger<UserService> logger, IMapper mapper, IHasher hasher)
        {
            _manager = manager;
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

                var result = new ServiceResult(false, "User didn't save");
                return result;
            }

            user.PasswordHash = _hasher.HashPassword(model.Password, user.Salt);
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogError("Function add didn't hash the password in the salt");
                _logger.LogTrace("Djelato.Services.Services.UserService.Add()");

                var result = new ServiceResult(false, "User didn't save");
                return result;
            }

            await _manager.UserManager.AddAsync(user);

            bool isSave = await _manager.UserManager.CheckAsync(model.Email);
            if(isSave)
            {
                var result = new ServiceResult(true, null);
                return result;
            }
            else
            {
                var result = new ServiceResult(false, "User didn't save");
                return result;
            }
        }

        public async Task<ServiceResult> CheckByEmailAsync(string email)
        {
            bool isExist = await _manager.UserManager.CheckAsync(email);
            if (isExist)
            {
                var result = new ServiceResult(true, "User exist");
                return result;
            }
            else
            {
                var result = new ServiceResult(false, "User nonexistent");
                return result;
            }
        }


    }
}
