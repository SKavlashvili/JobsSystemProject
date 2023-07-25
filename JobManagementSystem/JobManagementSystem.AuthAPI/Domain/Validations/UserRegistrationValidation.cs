using FluentValidation;
using JobManagementSystem.AuthAPI.Presentation;
namespace JobManagementSystem.AuthAPI.Domain
{
    public class UserRegistrationValidation : AbstractValidator<UserRegistrationModel>
    {
        public UserRegistrationValidation()
        {
            RuleFor((UserRegistrationModel newUser) => newUser.Email)
                .NotEmpty().EmailAddress();

            RuleFor(newUser => newUser.FirstName)
                .NotEmpty()
                .Must((string firstName) => firstName.Length < 100)
                .WithMessage("FirstName Length must be less then 100");

            RuleFor(newUser => newUser.LastName)
                .NotEmpty()
                .Must((string lastName) => lastName.Length < 100)
                .WithMessage("LastName Length must be less then 100");

            RuleFor(newUser => newUser)
                .NotEmpty()
                .Must((UserRegistrationModel newUser) =>
                {
                    bool isValid = newUser.Password.Length > 8;
                    //hash password and set to newUser
                    return isValid;
                })
                .WithMessage("Password Length must be more then 8");


            When(newUser => newUser.IsEmployer, () =>
            {
                RuleFor(newUser => newUser.CompanyName)
                .Must(companyName => companyName != null && companyName.Length < 100)
                .WithMessage("CompanyName Length must be less then 100 and not empty");
            });

            When(newUser => !newUser.IsEmployer, () =>
            {
                RuleFor(newUser => newUser.CompanyName)
                .Empty()
                .WithMessage("CompanyName must be empty when user is not employer");
            });


        }
    }
}
