namespace Shared.Common.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse Succeeded(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse Failed(string error)
    {
        return new ApiResponse
        {
            Success = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse Failed(List<string> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Message = errors.FirstOrDefault() ?? "Operation failed",
            Errors = errors
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Succeeded(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public new static ApiResponse<T> Failed(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    public new static ApiResponse<T> Failed(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = errors.FirstOrDefault() ?? "Operation failed",
            Errors = errors
        };
    }
}