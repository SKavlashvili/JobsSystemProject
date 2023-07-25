namespace JobManagementSystem.AuthAPI.Presentation
{
    public class UserRegistrationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmployer { get; set; }
        public string? CompanyName { get; set; }
    }
}
