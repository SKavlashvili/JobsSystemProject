namespace JobManagementSystem.API.Infrastructure
{
    public class CVModelForLeaderBoard
    {
        public int UserId { get; set; }
        public decimal UserScore { get; set; }
        public string PhoneNumber { get; set; }
        public string Education { get; set; }
        public string Degree { get; set; }
        public string Profession { get; set; }
        public DateTime EducationStarted { get; set; }
        public DateTime EducationEnded { get; set; }
    }
}
