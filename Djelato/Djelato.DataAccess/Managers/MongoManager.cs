using Djelato.DataAccess.Context;
using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.DataAccess.MongoRepositories;
using Djelato.DataAccess.MongoRepositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Managers
{
    public class MongoManager : IMongoManager
    {
        private readonly MongoContext<User> _userContext;

        private IUserRepo<User> _userRepo;

        public MongoManager(IMongoDatabase database)
        {
            _userContext = new MongoContext<User>(database, "Users");
            _userContext.Collection.Indexes.CreateOne(new CreateIndexModel<User>(new IndexKeysDefinitionBuilder<User>().Ascending(x => x.Email), new CreateIndexOptions { Unique = true }));
        }

        public IUserRepo<User> UserManager
        {
            get
            {
                if (_userRepo == null)
                {
                    _userRepo = new UserRepo<User>(_userContext);
                }
                return _userRepo;
            }
        }
    }
}
