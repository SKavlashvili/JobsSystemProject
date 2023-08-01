using FluentValidation;
using JobManagementSystem.AuthAPI.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCustomServices();

//Swagger
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseGlobalErrorHandler();

app.UseRouting();

//Swaggger
app.UseSwagger();
app.UseSwaggerUI();



app.UseEndpoints((IEndpointRouteBuilder endpoints) =>
{
    endpoints.MapControllers();
});


app.Run();
