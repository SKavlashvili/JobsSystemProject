using StackExchange.Redis;
using JobManagementSystem.API.Infrastructure.BackgroundWorkers.Models;
using System.Text.Json;

namespace JobManagementSystem.API.Infrastructure
{
    public class UsersConsumerWorker : BackgroundService
    {
        private readonly IDatabase _usersTransferer;
        private readonly IDatabase _redisCache;
        public UsersConsumerWorker(IRedisQueue messageBrokerWrapper,IRedisCache redisCache)
        {
            _usersTransferer = messageBrokerWrapper.UsersTransferer();
            _redisCache = redisCache.GetCache();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string? data = null;
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    data = await _usersTransferer.ListRightPopAsync("UsersTransfer");
                    if (data == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    string[] splittedData = data.Split('|');
                    NewUser User = JsonSerializer.Deserialize<NewUser>(splittedData[1]);
                    User.UserId = Convert.ToInt32(splittedData[0]);
                    try
                    {
                        //Ternar operator: condition ? value_if_true : value_if_false
                        await _redisCache.HashSetAsync(User.Email, new HashEntry[]
                        {
                            new HashEntry("UserId",User.UserId),
                            new HashEntry("FirstName",User.FirstName),
                            new HashEntry("LastName",User.LastName),
                            new HashEntry("Password",User.Password),
                            new HashEntry("IsEmployer",User.IsEmployer),
                            new HashEntry("CompanyName",User.CompanyName == null ? "" : User.CompanyName)
                        });

                    }catch(Exception ex)
                    {
                        await _usersTransferer.ListRightPushAsync("UsersTransfer", data);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }

            }
        }
    }
}
