using Dapper;
using JobManagementSystem.AuthAPI.Application;
using Npgsql;
using JobManagementSystem.AuthAPI.Domain;
using System.Text.Json;

namespace JobManagementSystem.AuthAPI.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly IConnectionFactory _postgreConnectionFactory;
        private readonly IConfiguration _config;
        public AuthService(PostgreSQLConnectionFactory postgreConnFac,IConfiguration config)
        {
            _postgreConnectionFactory = postgreConnFac;
            _config = config;
        }
        public async Task<int> RegisterNewUser(NewUserRequest newUser)
        {
            int res = 0;
            using(NpgsqlConnection conn = (NpgsqlConnection)(await _postgreConnectionFactory.CreateConnection()))
            {
                await conn.OpenAsync();
                using (NpgsqlTransaction transaction = conn.BeginTransaction())
                {
                    User? user = (await conn.QueryAsync<User>
                        ("SELECT * FROM users WHERE Email = @Email", new { Email = newUser.Email }, transaction)).SingleOrDefault();
                    if (user != null) throw new UserAlreadyExists();

                    await conn.QueryAsync("INSERT INTO" +
                        " users(first_name,last_name,email,password,is_employer,company_name)" +
                        "values(@FirstName,@LastName,@Email,@Password,@IsEmployer,@CompanyName)", newUser, transaction);

                    res = (await conn.QueryAsync<int>("SELECT user_id FROM users WHERE email = @Email", new { Email = newUser.Email},transaction)).Single();
                
                    await transaction.CommitAsync();

                }
            }
            //Add into message queue
            string model = JsonSerializer.Serialize(newUser);
            await File.AppendAllTextAsync(_config.GetConnectionString("EventSource"), $"ADDED|{res}|{model}\n");
            return res;
        }
    }
}
