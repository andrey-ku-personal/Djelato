using Djelato.Services.Models;
using Djelato.Services.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Djelato.Services.Services
{
    

    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _hostEnv;
        private string imageFomat = ".jpeg";

        public FileService(IHostingEnvironment hostEnv)
        {
            _hostEnv = hostEnv;
        }

        public async Task<ServiceResult<string>> SaveAvatarImg(IFormFile avatarImg, string folderPath = null)
        {
            if (avatarImg != null && avatarImg.Length > 0)
            {
                string folderName = Path.Combine(_hostEnv.WebRootPath, @"Avatars");
                if(folderPath != null)
                {
                    folderName = Path.Combine(folderName, folderPath);
                }

                string filepath = Path.Combine(folderName, (Guid.NewGuid() + imageFomat));
                Directory.CreateDirectory(folderName);

                using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    await avatarImg.CopyToAsync(fileStream);
                }

                return new ServiceResult<string>(true, filepath, "Avatar image was saved");
            }

            return new ServiceResult<string>(false, null, "Your avatar didn't saved, please try create profile again");
        }
    }
}
