using Dapper;
using JobManagementSystem.JobsService.Application;
using JobManagementSystem.JobsService.Domain;
using Npgsql;

namespace JobManagementSystem.JobsService.Infrastructure
{
    public class Seeder : ISeeder
    {
        private readonly IConfiguration _config;
        public Seeder(IConfiguration config)
        {
            _config = config;
        }
        public async Task SeedPreDefinedData()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_config.GetConnectionString("Postgre")))
            {
                //Skills
                string[] skills = Enum.GetNames(typeof(Skill));
                for(int i = 0; i < skills.Length; i++)
                {
                    await conn.ExecuteAsync("INSERT INTO skills( skill_name ) values (@Val) ON CONFLICT (skill_name) DO NOTHING", new { Val = skills[i]});
                }
                //Proffesion
                string[] proffesions = Enum.GetNames(typeof(Profession));
                for (int i = 0; i < proffesions.Length; i++)
                {
                    await conn.ExecuteAsync("INSERT INTO profession( proffesion_name ) values (@Val) ON CONFLICT (proffesion_name) DO NOTHING", new { Val = proffesions[i] });
                }
                //Degree
                string[] degrees = Enum.GetNames(typeof(Degree));
                for (int i = 0; i < degrees.Length; i++)
                {
                    await conn.ExecuteAsync("INSERT INTO degrees( degree_name ) values (@Val) ON CONFLICT (degree_name) DO NOTHING", new { Val = degrees[i] });
                }
                //Education
                string[] education = Enum.GetNames(typeof(Education));
                for (int i = 0; i < education.Length; i++)
                {
                    await conn.ExecuteAsync("INSERT INTO education( education_name ) values (@Val) ON CONFLICT (education_name) DO NOTHING", new { Val = education[i] });
                }
            }

        }
    }
}
