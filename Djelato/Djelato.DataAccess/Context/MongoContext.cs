using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Context
{
    internal class MongoContext<T>
    {
        private readonly IMongoDatabase _database = null;

        //name of the collection in database
        private readonly string _collectionName = null;

        internal MongoContext(IMongoDatabase database, string collectionName)
        {
            _database = database;
            _collectionName = collectionName;
        }

        internal IMongoCollection<T> Object
        {
            get
            {
                IMongoCollection<T> collection = _database.GetCollection<T>(_collectionName);
                return collection;
            }
        }
    }
}
