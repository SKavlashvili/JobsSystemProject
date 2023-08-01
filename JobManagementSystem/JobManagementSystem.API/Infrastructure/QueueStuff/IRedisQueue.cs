using StackExchange.Redis;

namespace JobManagementSystem.API.Infrastructure
{
    public interface IRedisQueue
    {
        IDatabase UsersTransferer();
        IDatabase JobsServiceQueue();
    }
}
