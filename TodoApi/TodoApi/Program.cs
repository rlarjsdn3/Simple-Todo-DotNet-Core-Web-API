using TodoApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();