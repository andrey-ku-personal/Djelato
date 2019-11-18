using Djelato.Services.JWT.Interfaces;
using Djelato.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult> AuthorizeAsync(AuthModel model);
        Task<bool> CheckUserAsync(string email);
        Task<ServiceResult<string>> GetTokenAsync(string email);
    }
}
