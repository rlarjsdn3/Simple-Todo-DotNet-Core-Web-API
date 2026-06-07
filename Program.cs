using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = 
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "TodoDb 연결 문자열을 찾을 수 없습니다."
    );

builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();