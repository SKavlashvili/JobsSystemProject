using JobManagementSystem.JobsService.Application;
using StackExchange.Redis;

namespace JobManagementSystem.JobsService.Infrastructure
{
    public class RedisMessageManager : IRedisMessageManager
    {
        private readonly IDatabase _jobsRelatedRedisInstance;
        public RedisMessageManager(IConfiguration configs)
        {
            _jobsRelatedRedisInstance = ConnectionMultiplexer.Connect(configs.GetConnectionString("CVAndJobsTransferer")).GetDatabase();
        }
        public IDatabase GetInstanceForJobRelatedQueues()
        {
            return _jobsRelatedRedisInstance;
        }
    }
}
