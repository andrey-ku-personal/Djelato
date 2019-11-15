using Djelato.DataAccess.Entity;
using Djelato.DataAccess.MongoRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Managers.Interfaces
{
    public interface IMongoManager
    {
        IUserRepo<User> UserManager { get; }
    }
}
