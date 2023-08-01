namespace JobManagementSystem.API.Infrastructure.BackgroundWorkers.Models
{
    public class NewUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmployer { get; set; }
        public string CompanyName { get; set; }

    }
}
