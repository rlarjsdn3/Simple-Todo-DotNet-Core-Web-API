using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Middlewares;
using TodoApi.Services;

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

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();