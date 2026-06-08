using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Services;

namespace TodoApi.Controllers;

/// <summary>
/// 할 일 데이터를 관리하는 API 컨트롤러입니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }


    /// <summary>
    /// 모든 할 일을 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TodoResponse>>> GetAll(
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TodoResponse> todos = 
            await _todoService.GetAllAsync(
                keyword,
                cancellationToken
            );

        return Ok(todos);
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
        TodoResponse? todo = 
            await _todoService.GetByIdAsync(
                id,
                cancellationToken
            );
    
        if (todo is null)
        {
            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        return Ok(todo);
    }

    /// <summary>
    /// 새로운 할 일을 등록합니다.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        TodoResponse todo = 
            await _todoService.CreateAsync(
                request,
                cancellationToken
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id = todo.Id },
                todo
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
        TodoResponse? todo =
            await _todoService.UpdateAsync(
                id,
                request,
                cancellationToken
            );

        if (todo is null)
        {
            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        return Ok(todo);
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
        bool deleted = await _todoService.DeleteAsync(
            id,
            cancellationToken
        );

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"{id}번 할 일을 찾을 수 없습니다."
            });
        }

        return NoContent();
    }
}