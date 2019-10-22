using Djelato.DataAccess.Context;
using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.DataAccess.Repository;
using Djelato.DataAccess.Repository.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Managers
{
    public class MongoRepoManager : IMongoRepoManager
    {
        private readonly MongoContext<User> _userContext;

        private IUserRepository<User> _userRepo;

        public MongoRepoManager(IMongoDatabase database)
        {
            _userContext = new MongoContext<User>(database, "Users");
            _userContext.Collection.Indexes.CreateOne(new CreateIndexModel<User>(new IndexKeysDefinitionBuilder<User>().Ascending(x => x.Email), new CreateIndexOptions { Unique = true }));
        }

        public IUserRepository<User> UserManager
        {
            get
            {
                if (_userRepo == null)
                {
                    _userRepo = new UserRepository<User>(_userContext);
                }
                return _userRepo;
            }
        }
    }
}
