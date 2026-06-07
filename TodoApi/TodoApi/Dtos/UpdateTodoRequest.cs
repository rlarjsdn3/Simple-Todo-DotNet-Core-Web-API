using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos;

/// <summary>
/// Todo 등록 요청 데이터를 나타냅니다.
/// </summary>
public class UpdateTodoRequest
{
    /// <summary>
    /// 수정할 할 일의 제목입니다.
    /// </summary>
    [Required(ErrorMessage = "할 일 제목은 필수입니다.")]
    [StringLength(
        100,
        MinimumLength = 1,
        ErrorMessage = "할 일 제목은 1자 이상 100자 이하로 입력해주세요."
    )]
    [RegularExpression(
        @"^(?!\s\s*$).+",
        ErrorMessage = "할 일 제목은 공백으로만 구성할 수 없습니다."
    )]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 수정할 완료 여부입니다.
    /// </summary>
    public bool IsCompleted { get; set; }
}