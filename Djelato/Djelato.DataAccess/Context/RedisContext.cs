using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.DataAccess.Context
{
    internal class RedisContext
    {
        private readonly IConnectionMultiplexer _redisCli = null;

        internal RedisContext(IConnectionMultiplexer redisCli)
        {
            _redisCli = redisCli;
        }

        internal IDatabase Database
        {
            get
            {
                IDatabase database = _redisCli.GetDatabase();
                return database;
            }
        }
    }
}
