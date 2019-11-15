using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.DataAccess.RedisRepositories.Interfaces
{
    public interface IRedisRepo
    {
        Task<string> GetAsync(string key);
        Task<bool> SetAsync(string key, string value);
        Task<bool> SetAsync(string key, string value, TimeSpan timeExpiration);
    }
}
