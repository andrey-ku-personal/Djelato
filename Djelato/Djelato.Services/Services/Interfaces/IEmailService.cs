using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Services.Interfaces
{
    public interface IEmailService
    {
        public Task CreateNotification(string email, Guid key);
    }
}
