using Djelato.DataAccess.Context;
using Djelato.DataAccess.Repository.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.DataAccess.Repository
{
    public  class RedisRepository : IRedisRepository
    {
        private readonly RedisContext _context;

        public RedisRepository(IConnectionMultiplexer connection)
        {
            _context = new RedisContext(connection);
        }

        public async Task<string> GetAsync(string key)
        {
            string result = await _context.Database.StringGetAsync(key);
            return result;
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            bool result = await _context.Database.StringSetAsync(key, value);
            return result;
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan timeExpiration)
        {
            bool result = await _context.Database.StringSetAsync(key, value, timeExpiration);
            return result;
        }
    }
}
