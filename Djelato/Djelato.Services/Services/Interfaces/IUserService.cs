using Djelato.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResult> CheckByEmailAsync(string email);
        Task<ServiceResult> AddAsync(UserModel model);

    }
}
