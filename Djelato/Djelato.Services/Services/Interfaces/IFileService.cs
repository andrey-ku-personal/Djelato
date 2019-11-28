using Djelato.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services.Interfaces
{
    public interface IFileService
    {
        Task<ServiceResult<string>> SaveAvatarImg(IFormFile avatarImg, string folderPath = null);
    }
}
