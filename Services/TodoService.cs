using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services;

/// <summary>
/// Todo 관련 기능을 처리합니다.
/// </summary>
public class TodoService : ITodoService
{
    private readonly TodoDbContext _dbContext;
    private readonly ILogger<TodoService> _logger;

    public TodoService(
        TodoDbContext dbContext,
        ILogger<TodoService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    } 

    /// <inheritdoc />
    public async Task<IReadOnlyList<TodoResponse>> GetAllAsync(
        string? keyword,
        CancellationToken cancellationToken)
    {
        IQueryable<Todo> query = _dbContext.Todos
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string trimmedKeyword = keyword.Trim();

            query = query.Where(todo =>
                todo.Title.Contains(trimmedKeyword)
            );
        }

        List<TodoResponse> todos = await query
            .OrderByDescending(todo => todo.Id)
            .Select(todo => new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return todos;
    } 

    /// <inheritdoc />
    public async Task<TodoResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        TodoResponse? todo = await _dbContext.Todos
            .AsNoTracking()
            .Where(todo => todo.Id == id)
            .Select(todo => new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return todo;
    }

    /// <inheritdoc />
    public async Task<TodoResponse> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            Title = request.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Todos.Add(todo);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 등록했습니다. TodoId={TodoId}, Title={Title}",
            todo.Id,
            todo.Title
        );

        return ToResponse(todo);
    }

    /// <inheritdoc />
    public async Task<TodoResponse?> UpdateAsync(
        int id, 
        UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        Todo? todo = await _dbContext.Todos
            .FirstOrDefaultAsync(
                todo => todo.Id == id,
                cancellationToken
            );

        if (todo is null)
        {
            return null;
        }

        todo.Title = request.Title.Trim();
        todo.IsCompleted = request.IsCompleted;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 수정했습니다. TodoId={TodoId}, Title={Title}, IsCompleted={IsCompleted}",
            todo.Id,
            todo.Title,
            todo.IsCompleted
        );

        return ToResponse(todo);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        Todo? todo = await _dbContext.Todos
            .FirstOrDefaultAsync(
                todo => todo.Id == id,
                cancellationToken
            );

        if (todo is null)
        {
            return false;
        }

        _dbContext.Todos.Remove(todo);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 삭제했습니다. TodoId={TodoId}, Title={Title}",
            todo.Id,
            todo.Title
        );

        return true;
    }

    private TodoResponse ToResponse(Todo todo)
    {
        return new TodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt
        };
    }
}