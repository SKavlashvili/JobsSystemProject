using FluentValidation;
using FluentValidation.Results;
using JobManagementSystem.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
namespace JobManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthPartialController : ControllerBase
    {
        private readonly IValidator<UserLoginModel> _userValidator;
        private readonly IDatabase _cache;
        private readonly IConfiguration _config;
        public AuthPartialController(IValidator<UserLoginModel> userValidator, IRedisCache cache, IConfiguration config)
        {
            _userValidator = userValidator;
            _cache = cache.GetCache();
            _config = config;
        }
        [HttpPost("/[action]")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginModel userInfo)
        {
            ValidationResult valRes = _userValidator.Validate(userInfo);
            if (!valRes.IsValid) return BadRequest(valRes.Errors);
            HashEntry[] cachedUser = await _cache.HashGetAllAsync(userInfo.Email);
            
            if (cachedUser.Length == 0) return BadRequest(new { Message = "Mail is incorrect" });

            HashEntry password = cachedUser.Single((entry) => entry.Name.Equals("Password"));
            HashEntry isEmployer = cachedUser.Single((entry) => entry.Name.Equals("IsEmployer"));
            HashEntry UserIdInfo = cachedUser.Single((entry) => entry.Name.Equals("UserId"));

            if (userInfo.Password.Equals(password.Value)) return Ok(new
            {
                Token = JWT.GenerateToken(_config.GetValue<string>("JWT:SecurityKey"),
                new Claim(ClaimTypes.Role, isEmployer.Value.Equals("0") ? "Applicant" : "Employer"),
                new Claim("UserID", UserIdInfo.Value))
            });
            
            return BadRequest(new {Message = "Password is incorrect"});
        }
    }
}
