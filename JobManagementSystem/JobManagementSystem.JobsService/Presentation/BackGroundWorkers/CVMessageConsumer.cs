using Dapper;
using JobManagementSystem.JobsService.Application;
using JobManagementSystem.JobsService.Domain;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json;

namespace JobManagementSystem.JobsService.Presentation
{
    public class CVMessageConsumer : BackgroundService
    {
        private readonly IDatabase _jobsRelatedRedisInstance;
        private readonly IConfiguration _config;
        public CVMessageConsumer(IRedisMessageManager redisMessageManager, IConfiguration config)
        {
            _jobsRelatedRedisInstance = redisMessageManager.GetInstanceForJobRelatedQueues();
            _config = config;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string message = null;
                    message = await _jobsRelatedRedisInstance.ListRightPopAsync("cvQueue");
                    if(message == null)
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
                                CVModel cv = JsonSerializer.Deserialize<CVModel>(splittedMessage[1]);
                                int UserID = Convert.ToInt32(splittedMessage[0]);

                                bool AlreadyIn = (await conn.QueryAsync<int?>($"SELECT user_id FROM cvs where user_id = {UserID} and is_deleted = false")).SingleOrDefault() != null;
                                
                                if (AlreadyIn)
                                {
                                    await conn.ExecuteAsync($"DELETE FROM cv_skill where user_id = {UserID}", new { }, transaction);
                                    await conn.ExecuteAsync($"DELETE FROM cvs WHERE user_id = {UserID}",new {},transaction);
                                }


                                string CVQuery = "INSERT INTO cvs " +
                                    "(user_id,phone_number,education,degree,profession,education_started,education_ended) " +
                                    "values(@UserID,@PhoneNumber,@Education,@Degree,@Profession,@EducationStarted,@EducationEnded)";
                                await conn.ExecuteAsync(CVQuery, new
                                {
                                    UserID = UserID,
                                    PhoneNumber = cv.PhoneNumber,
                                    Education = ((Education)cv.Education).ToString(),
                                    Degree = ((Degree)cv.Degree).ToString(),
                                    Profession = ((Profession)cv.Profession).ToString(),
                                    EducationStarted = cv.EducationStarted,
                                    EducationEnded = cv.EducationEnded
                                }, transaction);

                                string SkillsQuery = "INSERT INTO cv_skill" +
                                    "(user_id,skill_name,experience) " +
                                    "values (@UserID,@SkillName,@Exp)";
                                foreach(SkillDescriptor skill in cv.Skills)
                                {
                                    await conn.ExecuteAsync(SkillsQuery, new
                                    {
                                        UserID = UserID,
                                        SkillName = ((Skill)skill.skill).ToString(),
                                        Exp = skill.expirience
                                    }, transaction);
                                }
                                await transaction.CommitAsync();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _jobsRelatedRedisInstance.ListRightPushAsync("cvQueue",message);
                        await Task.Delay(1000);
                    }
                    
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
    }
}
