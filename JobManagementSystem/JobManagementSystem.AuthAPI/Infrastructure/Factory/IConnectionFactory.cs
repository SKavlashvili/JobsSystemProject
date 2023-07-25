namespace JobManagementSystem.AuthAPI.Infrastructure
{
    public interface IConnectionFactory
    {
        Task<object> CreateConnection();
    }
}
