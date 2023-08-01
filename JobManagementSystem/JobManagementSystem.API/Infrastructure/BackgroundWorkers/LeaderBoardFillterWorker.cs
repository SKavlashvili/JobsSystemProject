using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobManagementSystem.API.Infrastructure
{
    public class LeaderBoardFillterWorker : BackgroundService
    {
        private readonly IDatabase _redisCache;
        private readonly IDatabase _jobsServiceQueue;
        public LeaderBoardFillterWorker(IRedisCache redisCache, IRedisQueue redisQueueResolver)
        {
            _redisCache = redisCache.GetCache();
            _jobsServiceQueue = redisQueueResolver.JobsServiceQueue();
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string applicationMessage = null;
                    applicationMessage = await _jobsServiceQueue.ListRightPopAsync("usersForLeaderBoard");
                    if (applicationMessage == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    try
                    {
                        string[] splittedMessage = applicationMessage.Split('|');
                        JustScoreModel scoreModel = JsonSerializer.Deserialize<JustScoreModel>(splittedMessage[1]);
                        //splittedMessage[0] = JobID and splittedMessage[2] = EmployerID(owner of this vacancy)
                        await _redisCache.SortedSetAddAsync($"{splittedMessage[0]}|{splittedMessage[2]}", new SortedSetEntry[] { new SortedSetEntry(splittedMessage[1], (double)scoreModel.UserScore) });
                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _jobsServiceQueue.ListRightPushAsync("usersForLeaderBoard",applicationMessage);
                        await Task.Delay(1000);
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
