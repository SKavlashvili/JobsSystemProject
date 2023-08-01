using JobManagementSystem.JobsService.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobManagementSystem.JobsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsInfoController : ControllerBase
    {
        private readonly IJobsService _jobsService;
        public JobsInfoController(IJobsService jobsService)
        {
            _jobsService = jobsService;
        }
        [HttpGet("/[action]")]
        public async Task<IActionResult> GetAllJobs()
        {
            return Ok(await _jobsService.GetAllJobs());
        }

        [HttpGet("/[action]"), Authorize(Roles ="Employer")]
        public async Task<IActionResult> GetAllMyJobs()
        {
            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });

            int EmployerID = Convert.ToInt32(UserIDClaim.Value);
            return Ok(await _jobsService.GetAllMyJobs(EmployerID));
        }

        [HttpDelete("/[action]/{JobID}"), Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteJob(int JobID)
        {
            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });

            int EmployerID = Convert.ToInt32(UserIDClaim.Value);
            return Ok(new { Message = await _jobsService.DeleteJob(JobID, EmployerID) });
        }

    }
}
