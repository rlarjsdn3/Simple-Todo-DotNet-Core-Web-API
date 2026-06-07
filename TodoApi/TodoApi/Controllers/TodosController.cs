using Microsoft.AspNetCore.Mvc;
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
    /// <summary>
    /// 데이터베이스 대신 사용할 임시 할 일 목록입니다.
    /// </summary>
    private static readonly List<Todo> Todos = [
        new Todo { Id = 1, Title = "ASP.NET Core 공부하기", IsCompleted = false },
        new Todo { Id = 2, Title = "Todo API 만들기", IsCompleted = true },
        new Todo { Id = 3, Title = "문어와 흰둥이 이뻐해주기", IsCompleted = false }
    ];

    /// <summary>
    /// 새로운 할 일에 부여할 다음 번호입니다.
    /// </summary>
    private static int _nextId = 4;

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger<TodosController> _logger;

    /// <summary>
    /// 
    /// </summary>
    public TodosController(ILogger<TodosController> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// 모든 할 일을 조회합니다.
    /// </summary>
    [HttpGet]
    public ActionResult<List<TodoResponse>> GetAll(
        [FromQuery] string? keyword)
    {
        _logger.LogInformation(
            "Todo 목록 조회 요청: Keyword={Keyword}",
            keyword
        );

        if (string.IsNullOrWhiteSpace(keyword))
        {
            _logger.LogInformation(
                "전체 Todo 목록을 반환합니다. Count={TodoCount}",
                Todos.Count
            );

            return Ok(Todos);
        }

        List<TodoResponse> response = Todos
            .Where(todo => 
                todo.Title.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            .Select(ToResponse)
            .ToList();

        _logger.LogInformation(
            "Todo 검색이 완료되었습니다. Keyword={Keyword}, ResultCount={ResultCount}",
            keyword,
            response.Count
        );

        return Ok(response);
    }

    /// <summary>
    /// 번호에 해당하는 할 일을 조회합니다.
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id:int}")]
    public ActionResult<TodoResponse> GetById(int id)
    {
        _logger.LogInformation(
            "Todo 단일 조회 요청: TodoId={TodoId}",
            id
        );

        Todo? todo = Todos.FirstOrDefault(todo => todo.Id == id);

        if (todo is null)
        {
            _logger.LogWarning(
                "조회할 Todo를 찾지 못했습니다. TodoId={TodoId}",
                id
            );

            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        _logger.LogInformation(
            "Todo를 조회했습니다. TodoID={TodoId}, Title={Title}",
            todo.Id,
            todo.Title
        );

        return Ok(ToResponse(todo));
    }

    /// <summary>
    /// 새로운 할 일을 등록합니다.
    /// </summary>
    [HttpPost]
    public ActionResult<TodoResponse> Create([FromBody] CreateTodoRequest request)
    {
        _logger.LogInformation(
            "Todo 등록 요청: Title={Title}",
            request.Title
        );

        var todo = new Todo
        {
            Id = _nextId,
            Title = request.Title,
            IsCompleted = false
        };

        _nextId++;

        Todos.Add(todo);

        _logger.LogInformation(
            "Todo를 등록했습니다. TodoId={TodoId}, Title={Title}",
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
    public ActionResult<TodoResponse> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        _logger.LogInformation(
            "Todo 수정 요청: TodoId={TodoId}, Title={Title}, IsCompleted={IsCompleted}",
            id,
            request.Title,
            request.IsCompleted
        );

        Todo? todo = Todos.FirstOrDefault(todo => todo.Id == id);

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

        todo.Title = request.Title.Trim();
        todo.IsCompleted = request.IsCompleted;

        _logger.LogInformation(
            "Todo를 수정했습니다. TodoId={TodoId}, Title={Title}, IsCompeleted={IsCompleted}",
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
    public IActionResult Delete(int id)
    {
        _logger.LogInformation(
            "Todo 삭제 요청: TodoId={TodoId}",
            id
        );

        Todo? todo = Todos.FirstOrDefault(todo => todo.Id == id);

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

        Todos.Remove(todo);

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