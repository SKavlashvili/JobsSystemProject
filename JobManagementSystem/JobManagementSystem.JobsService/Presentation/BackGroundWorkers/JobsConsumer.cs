using Dapper;
using JobManagementSystem.JobsService.Application;
using JobManagementSystem.JobsService.Domain;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json;

namespace JobManagementSystem.JobsService.Presentation
{
    public class JobsConsumer : BackgroundService
    {
        private readonly IDatabase _jobsRelatedRedisInstance;
        private readonly IConfiguration _config;

        public JobsConsumer(IRedisMessageManager redisManager, IConfiguration config)
        {
            _jobsRelatedRedisInstance = redisManager.GetInstanceForJobRelatedQueues();
            _config = config;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string message = null;
                    message = await _jobsRelatedRedisInstance.ListRightPopAsync("jobsQueue");
                    if (message == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("Postgre")))
                        {
                            await conn.OpenAsync();
                            using (NpgsqlTransaction transaction = conn.BeginTransaction())
                            {
                                string[] splittedMessage = message.Split('|');
                                int EmployerID = Convert.ToInt32(splittedMessage[0]);
                                JobModel job = JsonSerializer.Deserialize<JobModel>(splittedMessage[1]);

                                string AddJobQuery = "INSERT INTO jobs " +
                                    "(user_id,expire_date,job_title) VALUES (@UserID,@ExpDate,@JobTitle)";
                                await conn.QueryAsync(AddJobQuery, new
                                {
                                    UserID = EmployerID,
                                    ExpDate = job.ExpireDate,
                                    JobTitle = job.JobTitle
                                }, transaction);

                                int JobID = (await conn.QueryAsync<int>("select job_id from jobs order by created desc limit 1")).Single();
                                string AddReqSkillsQuery = "INSERT INTO job_skills " +
                                    "(job_id,skill_name,requestedExpirience,weight) " +
                                    "values (@JobID,@SkillName,@RequestedExp,@Weight)";
                                foreach(JobSkillDescriptor skill in job.JobSkills)
                                {
                                    await conn.QueryAsync(AddReqSkillsQuery, new
                                    {
                                        JobID = JobID,
                                        SkillName = ((Skill)skill.skill).ToString(),
                                        RequestedExp = skill.requestedExpirience,
                                        Weight = skill.weight
                                    }, transaction);
                                }
                                await transaction.CommitAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _jobsRelatedRedisInstance.ListRightPushAsync("jobsQueue", message);
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
