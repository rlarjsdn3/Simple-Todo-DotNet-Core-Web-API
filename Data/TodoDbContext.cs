using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

/// <summary>
/// Todo 데이터베이스와 EF Core 사이의 작업을 관리합니다.
/// </summary>
public class TodoDbContext : DbContext
{
    /// <summary>
    /// 외부에서 전달받은 DB 설정으로 Context를 초기화합니다.
    /// </summary>
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Todos 테이블에 접근합니다.
    /// </summary>
    public DbSet<Todo> Todos => Set<Todo>();
}