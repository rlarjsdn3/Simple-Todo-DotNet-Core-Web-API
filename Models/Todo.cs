using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

/// <summary>
/// 데이터베이스에 저장할 할 일 정ㅂ를 나타냅니다.
/// </summary>
public class Todo
{
    /// <summary>
    /// 할 일의 고유 번호입니디.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 할 일의 제목입니다.
    /// </summary>
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 할 일의 완료 여부입니다.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 할 일을 등록한 UTC 시각입니다.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
} 