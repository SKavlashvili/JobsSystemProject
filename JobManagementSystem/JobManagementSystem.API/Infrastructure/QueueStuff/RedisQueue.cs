using StackExchange.Redis;

namespace JobManagementSystem.API.Infrastructure
{
    public class RedisQueue : IRedisQueue
    {
        private readonly ConnectionMultiplexer _usersTransferConnection;
        private readonly IDatabase _usersTransfer;
        private readonly ConnectionMultiplexer _jobsServiceConnection;
        private readonly IDatabase _jobsServiceQueue;

        public RedisQueue(IConfiguration configuration)
        {
            _usersTransferConnection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("UsersService"));
            _usersTransfer = _usersTransferConnection.GetDatabase();
            _jobsServiceConnection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("JobsService"));
            _jobsServiceQueue = _jobsServiceConnection.GetDatabase();
        }
        public IDatabase UsersTransferer()
        {
            return _usersTransfer;
        }
        public IDatabase JobsServiceQueue()
        {
            return _jobsServiceQueue;
        }
    }
}
