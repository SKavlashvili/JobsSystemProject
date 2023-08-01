using JobManagementSystem.JobsService.Infrastructure;

namespace JobManagementSystem.JobsService.Application
{
    public interface IJobsService
    {
        Task<List<JobModel>> GetAllJobs();
        Task<List<JobModel>> GetAllMyJobs(int EmployerID);

        Task<bool> DeleteJob(int JobID, int EmployerID);
    }
}
