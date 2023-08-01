namespace JobManagementSystem.JobsService.Infrastructure
{
    public class JobModel
    {
        public int JobId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpireDate { get; set; }
        public string JobTitle { get; set; }
        public List<string> Skills { get; set; }
    }
}
