using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Notification
{
    public interface INotifier
    {
        public Task SendKey(string contact, int key);
    }
}
 