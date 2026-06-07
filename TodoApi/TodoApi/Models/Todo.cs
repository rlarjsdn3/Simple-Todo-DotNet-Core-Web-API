namespace TodoApi.Models;

/// <summary>
/// 하나의 할 일 정보를 나타냅니다.
/// </summary>
public class Todo
{
    /// <summary>
    /// 할 일의 고유 번호입니디.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 할 일의 제목입니다.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 할 일의 완료 여부입니다.
    /// </summary>
    public bool IsCompleted { get; set; }
} 