using Dapper;
using JobManagementSystem.AuthAPI.Application;
using Npgsql;
using JobManagementSystem.AuthAPI.Domain;
using System.Text.Json;
using StackExchange.Redis;

namespace JobManagementSystem.AuthAPI.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly IConnectionFactory _postgreConnectionFactory;
        private readonly IConfiguration _config;
        private readonly IDatabase _redisDB;
        public AuthService(PostgreSQLConnectionFactory postgreConnFac,IConfiguration config,IDatabase redisDB)
        {
            _postgreConnectionFactory = postgreConnFac;
            _config = config;
            _redisDB = redisDB;
        }
        public async Task<int> RegisterNewUser(NewUserRequest newUser)
        {
            int res = 0;//res is new user's ID in database
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
                    
                    //Add into message queue
                    string model = JsonSerializer.Serialize(newUser);
                    string UserData = $"{res}|{model}";
                    await _redisDB.ListLeftPushAsync("UsersTransfer", UserData);
                    //await File.AppendAllTextAsync(_config.GetConnectionString("EventSource"), $"ADDED|{UserData}\n");

                    await transaction.CommitAsync();

                }
            }
            return res;
        }
    }
}
