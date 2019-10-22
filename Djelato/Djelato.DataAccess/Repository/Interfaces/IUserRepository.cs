using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.DataAccess.Repository.Interfaces
{
    public interface IUserRepository<T> where T : class
    {
        Task AddAsync(T obj);
        Task<bool> CheckAsync(string email);
        Task<T> GetAsync(string email);
        Task<ReplaceOneResult> ReplaceOneAsync(string id, T obj);
    }
}
