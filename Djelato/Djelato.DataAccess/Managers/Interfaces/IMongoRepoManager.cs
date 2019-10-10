using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Managers.Interfaces
{
    public interface IMongoRepoManager
    {
        IUserRepository<User> UserManager { get; }
    }
}
