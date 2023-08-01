using FluentValidation;
using JobManagementSystem.API.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Services
builder.Services.AddSingleton<IRedisQueue, RedisQueue>();
builder.Services.AddSingleton<IRedisCache, RedisCache>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Auth
builder.Services.AddAuthentication()
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


//Reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


//Swagger
builder.Services.AddSwaggerGen();

//Workers
builder.Services.AddHostedService<UsersConsumerWorker>();
builder.Services.AddHostedService<LeaderBoardFillterWorker>();

var app = builder.Build();


app.UseRouting();

//Error Handler
app.UseGlobalErrorHandler();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Auth
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints((IEndpointRouteBuilder endpoints) =>
{
    endpoints.MapControllers();
    endpoints.MapReverseProxy();
});

app.Run();
