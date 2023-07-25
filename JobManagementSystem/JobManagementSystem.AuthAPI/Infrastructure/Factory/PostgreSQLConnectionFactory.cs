using Npgsql;

namespace JobManagementSystem.AuthAPI.Infrastructure
{
    public class PostgreSQLConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _config;
        public PostgreSQLConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public async Task<object> CreateConnection()
        {
            return await Task.FromResult((object)new NpgsqlConnection(_config.GetConnectionString("Postgre")));
        }
    }
}
