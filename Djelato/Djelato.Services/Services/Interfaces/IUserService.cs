using Djelato.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult> AddAsync(UserModel model);
        public Task<bool> ConfirmEmailAsync(string email);
        public Task<bool> CheckByEmailAsync(string email);
    }
}
