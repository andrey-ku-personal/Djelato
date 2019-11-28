using Djelato.DataAccess.Entity;
using Djelato.DataAccess.MongoRepositories.Interfaces;

namespace Djelato.DataAccess.Managers.Interfaces
{
    public interface IMongoManager
    {
        IUserRepo<User> UserManager { get; }
    }
}
