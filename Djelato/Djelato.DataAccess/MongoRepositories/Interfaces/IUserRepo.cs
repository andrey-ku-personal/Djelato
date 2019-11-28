using MongoDB.Driver;
using System.Threading.Tasks;

namespace Djelato.DataAccess.MongoRepositories.Interfaces
{
    public interface IUserRepo<T> where T : class
    {
        Task AddAsync(T obj);
        Task<bool> CheckAsync(string email);
        Task<T> GetAsync(string email);
        Task<ReplaceOneResult> ReplaceOneAsync(string id, T obj);
    }
}
