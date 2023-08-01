using Microsoft.AspNetCore.Authorization;
using JobManagementSystem.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;
using System.Security.Claims;

namespace JobManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IDatabase _jobsServiceQueue;
        private readonly IValidator<CVModel> _cvValidator;
        private readonly IValidator<JobModel> _jobValidator;
        private readonly IDatabase _cache;
        public JobsController(IRedisQueue queueResolver,IValidator<CVModel> cvValidator, IValidator<JobModel> jobValidator, IRedisCache redisCache)
        {
            _jobsServiceQueue = queueResolver.JobsServiceQueue();
            _cvValidator = cvValidator;
            _jobValidator = jobValidator;
            _cache = redisCache.GetCache();
        }

        [HttpPost("/[action]"), Authorize(Roles = "Applicant", AuthenticationSchemes = "MainSchema")]
        public async Task<IActionResult> AddCV([FromBody] CVModel cv)
        {
            ValidationResult valRes = _cvValidator.Validate(cv);
            if (!valRes.IsValid) return BadRequest(valRes.Errors);

            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });
            
            int UserID = Convert.ToInt32(UserIDClaim.Value);

            string MessageForQueue = $"{UserID}|{JsonSerializer.Serialize<CVModel>(cv)}";
            await _jobsServiceQueue.ListLeftPushAsync("cvQueue", MessageForQueue);

            return Ok(new {Message = "CV Added"});
        }

        [HttpPut("/[action]"), Authorize(Roles = "Applicant", AuthenticationSchemes = "MainSchema")]
        public async Task<IActionResult> UpdateCV([FromBody] CVModel cv)
        {
            ValidationResult valRes = _cvValidator.Validate(cv);
            if (!valRes.IsValid) return BadRequest(valRes.Errors);

            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });

            int UserID = Convert.ToInt32(UserIDClaim.Value);

            string MessageForQueue = $"{UserID}|{JsonSerializer.Serialize<CVModel>(cv)}";
            await _jobsServiceQueue.ListLeftPushAsync("cvQueue", MessageForQueue);

            return Ok(new { Message = "CV Added" });
        }

        [HttpPost("/[action]"),Authorize(AuthenticationSchemes="MainSchema",Roles ="Employer")]
        public async Task<IActionResult> AddJobDescription([FromBody] JobModel job)
        {
            ValidationResult valRes = _jobValidator.Validate(job);
            if (!valRes.IsValid) return BadRequest(valRes.Errors);

            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });


            int EmployerID = Convert.ToInt32(UserIDClaim.Value);

            string MessageForQueue = $"{EmployerID}|{JsonSerializer.Serialize<JobModel>(job)}";
            await _jobsServiceQueue.ListLeftPushAsync("jobsQueue", MessageForQueue);

            return Ok(new {Message = "Job added"});
        }



        [HttpPost("/[action]/{JobID}"),Authorize(AuthenticationSchemes ="MainSchema",Roles ="Applicant")]
        public async Task<IActionResult> SendApplication(int JobID)
        {
            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });


            int UserID = Convert.ToInt32(UserIDClaim.Value);

            await _jobsServiceQueue.ListLeftPushAsync("applicationsQueue", $"{JobID}|{UserID}");
            return Ok(new {Message = "Application added"});
        }

        [HttpGet("/[action]/{JobID}") , Authorize(AuthenticationSchemes ="MainSchema",Roles ="Employer")]
        public async Task<IActionResult> GetMyJobLeaderBoard(int JobID)
        {
            Claim UserIDClaim = User.FindFirst(claim => claim.Type.Equals("UserID"));
            if (UserIDClaim == null) return BadRequest(new { Message = "Not authorized" });


            int EmployerID = Convert.ToInt32(UserIDClaim.Value);

            //Redis Key: {JobID}|{EmployerID}
            RedisValue[] applicants = (await _cache.SortedSetRangeByRankAsync($"{JobID}|{EmployerID}", 0, -1, Order.Descending));
            List<CVModelForLeaderBoard> leaderBoard = new List<CVModelForLeaderBoard>();
            for(int i = 0; i < applicants.Length; i++)
            {
                leaderBoard.Add(JsonSerializer.Deserialize<CVModelForLeaderBoard>(applicants[i]));
            }
            return Ok(leaderBoard);
        }



        [HttpGet("/[action]")]
        public IActionResult GetAllPredefinedData()
        {
            return Ok(new PredefinedDataResponseModel());
        }
    }
}
