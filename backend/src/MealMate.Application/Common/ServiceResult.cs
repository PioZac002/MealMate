namespace MealMate.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult<T> Ok(T data) =>
        new() { Success = true, Data = data, StatusCode = 200 };

    public static ServiceResult<T> Created(T data) =>
        new() { Success = true, Data = data, StatusCode = 201 };

    public static ServiceResult<T> Fail(string error, int statusCode = 400) =>
        new() { Success = false, Error = error, StatusCode = statusCode };

    public static ServiceResult<T> NotFound(string error = "Resource not found") =>
        new() { Success = false, Error = error, StatusCode = 404 };

    public static ServiceResult<T> Unauthorized(string error = "Unauthorized") =>
        new() { Success = false, Error = error, StatusCode = 401 };

    public static ServiceResult<T> Forbidden(string error = "Forbidden") =>
        new() { Success = false, Error = error, StatusCode = 403 };
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
