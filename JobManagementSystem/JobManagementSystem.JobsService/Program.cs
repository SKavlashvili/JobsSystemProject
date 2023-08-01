using JobManagementSystem.JobsService.Application;
using JobManagementSystem.JobsService.Infrastructure;
using JobManagementSystem.JobsService.Presentation;
using JobManagementSystem.JobsService.Presentation.BackGroundWorkers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
//TODO: 1) delete expired jobs 2) update cv

var builder = WebApplication.CreateBuilder(args);

//Custom Services
builder.Services.AddSingleton<IRedisMessageManager, RedisMessageManager>();
builder.Services.AddSingleton<IJobsService, JobsService>();

//BackgroundServices
builder.Services.AddHostedService<CVMessageConsumer>();
builder.Services.AddHostedService<JobsConsumer>();
builder.Services.AddHostedService<ApplicationsConsumer>();



builder.Services.AddControllers();

builder.Services.AddAuthentication("MainSchema")
    .AddJwtBearer("MainSchema", (JwtBearerOptions options) => {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey
            (
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecurityKey"))
            ),
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = async (MessageReceivedContext messageContext) =>
            {
                HttpRequest req = messageContext.Request;
                if (req.Headers.ContainsKey("Authorization"))
                    req.Headers["Authorization"] = "Bearer " + req.Headers["Authorization"];
            }
        };
    });

var app = builder.Build();

//Seed sql
ISeeder seeder = new Seeder(builder.Configuration);
await seeder.SeedPreDefinedData();


app.UseRouting();

//TODO: Add Global Error hanlder
app.UseMiddleware<GlobalErrorHandler>();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints((IEndpointRouteBuilder endpoints) =>
{
    endpoints.MapControllers();
});

app.Run();
