namespace Shared.Common.Models;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; protected set; } = string.Empty;
    public List<string> Errors { get; protected set; } = new();

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;

        if (!string.IsNullOrEmpty(error))
        {
            Errors.Add(error);
        }
    }

    protected Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<string>();
        Error = errors?.FirstOrDefault() ?? string.Empty;
    }

    public static Result Success() => new(true, string.Empty);

    public static Result Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be empty", nameof(error));

        return new Result(false, error);
    }

    public static Result Failure(List<string> errors)
    {
        if (errors == null || !errors.Any())
            throw new ArgumentException("Error list cannot be empty", nameof(errors));

        return new Result(false, errors);
    }

    public static Result Failure(params string[] errors)
    {
        return Failure(errors.ToList());
    }
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    protected Result(bool isSuccess, T? data, string error)
        : base(isSuccess, error)
    {
        Data = data;
    }

    protected Result(bool isSuccess, T? data, List<string> errors)
        : base(isSuccess, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return new Result<T>(true, data, string.Empty);
    }

    public new static Result<T> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be empty", nameof(error));

        return new Result<T>(false, default, error);
    }

    public new static Result<T> Failure(List<string> errors)
    {
        if (errors == null || !errors.Any())
            throw new ArgumentException("Error list cannot be empty", nameof(errors));

        return new Result<T>(false, default, errors);
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return Failure(errors.ToList());
    }

    public Result ToResult()
    {
        return IsSuccess
            ? Result.Success()
            : Result.Failure(Errors);
    }
}