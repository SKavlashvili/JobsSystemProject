using StackExchange.Redis;

namespace JobManagementSystem.API.Infrastructure
{
    public interface IRedisCache
    {
        IDatabase GetCache();
    }
}
