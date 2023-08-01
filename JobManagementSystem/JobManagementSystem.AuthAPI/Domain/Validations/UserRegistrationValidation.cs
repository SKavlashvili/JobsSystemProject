using FluentValidation;
using JobManagementSystem.AuthAPI.Presentation;
using System.Text;
using System.Security.Cryptography;

namespace JobManagementSystem.AuthAPI.Domain
{
    public class UserRegistrationValidation : AbstractValidator<UserRegistrationModel>
    {
        public static string HashString(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public UserRegistrationValidation()
        {
            RuleFor((UserRegistrationModel newUser) => newUser.Email)
                .NotNull().NotEmpty().EmailAddress();

            RuleFor(newUser => newUser.FirstName)
                .NotNull()
                .NotEmpty()
                .Must((string firstName) => firstName.Length < 100)
                .WithMessage("FirstName Length must be less then 100");

            RuleFor(newUser => newUser.LastName)
                .NotNull()
                .NotEmpty()
                .Must((string lastName) => lastName.Length < 100)
                .WithMessage("LastName Length must be less then 100");

            RuleFor(newUser => newUser)
                .NotNull()
                .NotEmpty()
                .Must((UserRegistrationModel newUser) =>
                {
                    bool isValid = newUser.Password != null && newUser.Password.Length > 8;
                    newUser.Password = newUser.Password != null ? HashString(newUser.Password) : null;
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
