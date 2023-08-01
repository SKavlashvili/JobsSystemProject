using FluentValidation;
using FluentValidation.Results;
using JobManagementSystem.AuthAPI.Application;
using Microsoft.AspNetCore.Mvc;
using Mapster;

namespace JobManagementSystem.AuthAPI.Presentation
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IValidator<UserRegistrationModel> _newUserValidator;
        private readonly IAuthService _authService;
        public AuthController(IValidator<UserRegistrationModel> newUserValidator,IAuthService authService)
        {
            _newUserValidator = newUserValidator;
            _authService = authService;
        }
        [HttpPost("/[action]")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModel newUser)
        {
            ValidationResult valResult = _newUserValidator.Validate(newUser);
            if (!valResult.IsValid) return BadRequest(valResult.Errors);


            return Ok(new { AddedUserID = await _authService.RegisterNewUser(newUser.Adapt<NewUserRequest>())});
        }

    }
}
