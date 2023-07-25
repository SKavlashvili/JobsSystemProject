using JobManagementSystem.AuthAPI.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCustomServices();

builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();


app.UseEndpoints((IEndpointRouteBuilder endpoints) =>
{
    endpoints.MapControllers();
});


app.Run();
