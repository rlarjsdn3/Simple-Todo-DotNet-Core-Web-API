namespace TodoApi.Dtos;

/// <summary>
/// 클라이언트에 반환할 Todo 데이터를 나타냅니다.
/// </summary>
public class TodoResponse
{
    /// <summary>
    /// 할 일의 고유 번호입니다.
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