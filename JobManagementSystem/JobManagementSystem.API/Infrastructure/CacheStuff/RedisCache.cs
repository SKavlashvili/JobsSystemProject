using StackExchange.Redis;

namespace JobManagementSystem.API.Infrastructure
{
    public class RedisCache : IRedisCache
    {
        private readonly ConnectionMultiplexer _conn;
        private readonly IDatabase _cache;
        public RedisCache(IConfiguration config)
        {
            _conn = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisCache"));
            _cache = _conn.GetDatabase();
        }

        public IDatabase GetCache()
        {
            return _cache;
        }
    }
}
