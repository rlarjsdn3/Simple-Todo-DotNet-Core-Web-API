using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers;

/// <summary>
/// 할 일 데이터를 관리하는 API 컨트롤러입니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly TodoDbContext _dbContext;

    private readonly ILogger<TodosController> _logger;

    public TodosController(
        TodoDbContext dbContext,
        ILogger<TodosController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    /// <summary>
    /// 모든 할 일을 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TodoResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Todo 목록 조회 요청"
        );

        List<TodoResponse> response = await _dbContext.Todos
            .AsNoTracking()
            .OrderByDescending(todo => todo.Id)
            .Select(todo => new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// 번호에 해당하는 할 일을 조회합니다.
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Todo 단일 조회 요청: TodoId={TodoId}",
            id
        );

        Todo? todo = await _dbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(
                todo => todo.Id == id,
                cancellationToken
            );

        if (todo is null)
        {
            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        return Ok(ToResponse(todo));
    }

    /// <summary>
    /// 새로운 할 일을 등록합니다.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Todo 등록 요청: Title={Title}",
            request.Title
        );

        var todo = new Todo
        {
            Title = request.Title.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Todos.Add(todo);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 DB에 등록했습니다. TodoId={TodoId}, Title={Title}",
            todo.Id,
            todo.Title
        );

        TodoResponse response = ToResponse(todo);

        return CreatedAtAction(
            nameof(GetById),
            new { id = todo.Id },
            response
        );
    }

    /// <summary>
    /// 번호에 해당하는 할 일을 수정핣니다.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoResponse>> Update(
        int id, 
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Todo 수정 요청: TodoId={TodoId}, Title={Title}, IsCompleted={IsCompleted}",
            id,
            request.Title,
            request.IsCompleted
        );

        Todo? todo = await _dbContext.Todos
            .FirstOrDefaultAsync(
                todo => todo.Id == id,
                cancellationToken
            );

        if (todo is null)
        {
            _logger.LogWarning(
                "수정할 Todo를 찾지 못했습니다. TodoId={TodoId}",
                id
            );

            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        todo.Title = request.Title; 
        todo.IsCompleted = request.IsCompleted;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 수정했습니다. TodoId={TodoId}, Title={Title}, IsCompleted={IsCompleted}",
            todo.Id,
            todo.Title,
            todo.IsCompleted
        );

        return Ok(ToResponse(todo));
    }

    /// <summary>
    /// 번호에 해당하는 할 일을 삭제합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Todo 삭제 요청: TodoId={TodoId}",
            id
        );

        Todo? todo = await _dbContext.Todos
            .FirstOrDefaultAsync(
                todo => todo.Id == id,
                cancellationToken
            );

        if (todo is null)
        {
            _logger.LogWarning(
                "삭제할 Todo를 찾지 못했습니다. TodoId={TodoId}",
                id
            );

            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        _dbContext.Todos.Remove(todo);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Todo를 삭제했습니다. TodoId={TodoId}, Title={Title}",
            todo.Id,
            todo.Title
        );

        return NoContent();
    }

    /// <summary>
    /// 내부 Todo 모델을 API 응답 DTO로 변환합니다.
    /// </summary>
    private TodoResponse ToResponse(Todo todo)
    {
        return new TodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };
    }
}