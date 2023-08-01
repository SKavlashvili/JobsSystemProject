using FluentValidation;
using FluentValidation.Results;
using System.Text;
using System.Security.Cryptography;

namespace JobManagementSystem.API.Infrastructure
{
    public class UserLoginValidator : AbstractValidator<UserLoginModel>
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

        public UserLoginValidator()
        {
            RuleFor((UserLoginModel user) => user.Email)
                .NotNull().NotEmpty().EmailAddress();

            RuleFor((UserLoginModel user) => user)
                .Must((user) =>
                {
                    bool isValid = user.Password != null && user.Password.Length > 8;
                    user.Password = user.Password != null ? HashString(user.Password) : null;
                    return isValid;
                }).WithMessage("User password length must be more then 8");
        }
    }
}
