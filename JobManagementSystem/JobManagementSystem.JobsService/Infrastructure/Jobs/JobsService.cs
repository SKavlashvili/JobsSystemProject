using Dapper;
using JobManagementSystem.JobsService.Application;
using JobManagementSystem.JobsService.Domain;
using JobManagementSystem.JobsService.Presentation;
using Npgsql;

namespace JobManagementSystem.JobsService.Infrastructure
{
    public class JobsService : IJobsService
    {
        private readonly IConfiguration _config;
        public JobsService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> DeleteJob(int JobID, int EmployerID)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("Postgre")))
            {
                await conn.OpenAsync();
                using(NpgsqlTransaction transaction = conn.BeginTransaction())
                {
                    JobModel? job = (await conn.QueryAsync<JobModel>("SELECT job_id as JobId, user_id as UserId, expire_date as ExpireDate,job_title as JobTitle FROM jobs WHERE job_id = @JobID AND user_id = @EmployerID", new
                    {
                        JobID = JobID,
                        EmployerID = EmployerID
                    }, transaction)).SingleOrDefault();

                    if (job == null) return false;


                    List<SkillDescriptorForDeletion> skills = (await conn.QueryAsync<SkillDescriptorForDeletion>("SELECT skill_name as skill,requestedexpirience as expirience FROM job_skills where job_id = @JobID", new
                    {
                        JobID = job.JobId
                    }, transaction)).ToList();


                    await conn.ExecuteAsync("DELETE FROM job_skills WHERE job_id = @JobID", new
                    {
                        JobID = job.JobId
                    }, transaction);

                    for(int i = 0; i < skills.Count; i++)
                    {
                        await conn.ExecuteAsync("INSERT INTO deleted_jobs_skills(job_id,skill_name,expirience) " +
                            "values(@JobID,@SkillName,@Exp)", new
                            {
                                JobID = JobID,
                                SkillName = skills[i].skill,
                                Exp = skills[i].expirience
                            }, transaction);
                    }

                    await conn.ExecuteAsync("DELETE FROM jobs WHERE job_id = @JobID AND user_id = @EmployerID", new
                    {
                        JobID = JobID,
                        EmployerID = EmployerID
                    }, transaction);

                    await conn.ExecuteAsync("INSERT INTO deleted_jobs(job_id,user_id,expire_date,job_title) " +
                        "values(@JobID,@UserID,@ExpireDate,@JobTitle)", new
                        {
                            JobID = job.JobId,
                            UserID = job.UserId,
                            ExpireDate = job.ExpireDate,
                            JobTitle = job.JobTitle
                        }, transaction);
                    
                    await transaction.CommitAsync();

                    return true;
                }
            }
        }

        public async Task<List<JobModel>> GetAllJobs()
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("Postgre")))
            {
                await conn.OpenAsync();
                using(NpgsqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    List<JobModel> jobs = (await conn.QueryAsync<JobModel>("select job_id as JobId, user_id as UserId, expire_date as ExpireDate, job_title as JobTitle from jobs;", new {} ,transaction)).ToList();

                    for(int i = 0; i < jobs.Count; i++)
                    {
                        jobs[i].Skills = (await conn.QueryAsync<string>($"select skill_name from job_skills where job_id = {jobs[i].JobId};", new { }, transaction)).ToList();
                    }
                    return jobs;
                }
            }
        }
        public async Task<List<JobModel>> GetAllMyJobs(int EmployerID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("Postgre")))
            {
                await conn.OpenAsync();
                using (NpgsqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    List<JobModel> jobs = (await conn.QueryAsync<JobModel>($"select job_id as JobId, user_id as UserId, expire_date as ExpireDate, job_title as JobTitle from jobs where user_id = {EmployerID};", new { }, transaction)).ToList();

                    for (int i = 0; i < jobs.Count; i++)
                    {
                        jobs[i].Skills = (await conn.QueryAsync<string>($"select skill_name from job_skills where job_id = {jobs[i].JobId};", new { }, transaction)).ToList();
                    }
                    return jobs;
                }
            }
        }
    }
}
