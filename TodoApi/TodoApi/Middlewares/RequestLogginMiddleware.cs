using System.Diagnostics;

namespace TodoApi.Middlewares;

/// <summary>
/// 모든 HTTP 요청과 응답 정보를 기록합니다.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        string method = context.Request.Method;
        string path = context.Request.Path;

        _logger.LogInformation(
            "HTTP 요청 시작: Method={Method}, Path={Path}",
            method,
            path
        );

        try
        {
            // 다음 미들웨어 또는 Controller를 실행합니다.
            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP 요청 완료: Method={Method}, Path={Path}, StatusCode={StatusCode}, ElapsedMs={ElapsedMs}",
                method,
                path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "HTTP 요청 처리 중 오류 발생: Method={Method}, ElapsedMs={ElapsedMs}",
                method,
                stopwatch.ElapsedMilliseconds
            );

            // 예외를 여기서 없애지 않고 다시 위로 전달합니다.
            throw;
        }
    }
}