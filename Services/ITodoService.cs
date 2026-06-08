using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services;

/// <summary>
/// Todo 관련 기능을 정의합니다.
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Todo 목록을 조회합니다.
    /// </summary>
    Task<IReadOnlyList<TodoResponse>> GetAllAsync(
        string? keyword,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// ID에 해당하는 Todo를 조회합니다.
    /// </summary>
    Task<TodoResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// 새로운 Todo를 등록합니다.
    /// </summary>
    Task<TodoResponse> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// ID에 해당하는 Todo를 수정합니다.
    /// </summary>
    Task<TodoResponse?> UpdateAsync(
        int id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// ID에 해당하는 Todo를 삭제합니다.
    /// </summary>
    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken
    );
}