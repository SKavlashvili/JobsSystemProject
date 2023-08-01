using StackExchange.Redis;

namespace JobManagementSystem.JobsService.Application
{
    public interface IRedisMessageManager
    {
        IDatabase GetInstanceForJobRelatedQueues();
    }
}
