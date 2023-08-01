using Dapper;
using JobManagementSystem.JobsService.Application;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json;

namespace JobManagementSystem.JobsService.Presentation.BackGroundWorkers
{
    public class ApplicationsConsumer : BackgroundService
    {
        private readonly IDatabase _jobsRelatedRedisInstance;
        private readonly IConfiguration _config;
        public ApplicationsConsumer(IRedisMessageManager redisManager, IConfiguration config)
        {
            _config = config;
            _jobsRelatedRedisInstance = redisManager.GetInstanceForJobRelatedQueues();
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string message = null;
                    message = await _jobsRelatedRedisInstance.ListRightPopAsync("applicationsQueue");
                    if (message == null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    try
                    {
                        using(NpgsqlConnection conn =  new NpgsqlConnection(_config.GetConnectionString("Postgre")))
                        {
                            await conn.OpenAsync();
                            using(NpgsqlTransaction transaction = conn.BeginTransaction())
                            {
                                string[] splitted = message.Split('|');
                                int UserID = Convert.ToInt32(splitted[1]);
                                int JobID = Convert.ToInt32(splitted[0]);
                                int EmployerID = (await conn.QueryAsync<int>("select user_id from jobs where job_id = @JobID;",new {JobID = JobID})).Single();
                                string query = $@"SELECT 
	                                                SCORED_DATA.user_id as UserId,
	                                                SCORED_DATA.user_score as UserScore,
	                                                u.phone_number as PhoneNumber,
	                                                u.education as Education,
	                                                u.degree as Degree,
	                                                u.profession as Profession,
	                                                u.education_started as EducationStarted,
	                                                u.education_ended as EducationEnded
                                                FROM 
                                                (
	                                                SELECT 
	                                                  c.user_id AS user_id, 
	                                                  SUM((CAST(c.experience AS DECIMAL)/ j.requestedexpirience) * weight) AS user_score
	                                                FROM 
	                                                  cv_skill AS c 
	                                                  INNER JOIN job_skills as j on c.skill_name = j.skill_name 
	                                                  AND j.job_id = {JobID} 
	                                                WHERE 
	                                                  c.user_id = {UserID} AND c.is_deleted = false GROUP BY c.user_id
                                                ) AS SCORED_DATA
                                                INNER JOIN
                                                cvs AS u
                                                ON u.user_id = SCORED_DATA.user_id;";

                                CVModelForApplications ThisUser = (await conn.QueryAsync<CVModelForApplications>(query,new { },transaction)).SingleOrDefault();
                                if (ThisUser == null) continue;

                                await _jobsRelatedRedisInstance.ListLeftPushAsync("usersForLeaderBoard", $"{JobID}|{JsonSerializer.Serialize(ThisUser)}|{EmployerID}");
                            }
                        }
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _jobsRelatedRedisInstance.ListRightPushAsync("applicationsQueue", message);
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
