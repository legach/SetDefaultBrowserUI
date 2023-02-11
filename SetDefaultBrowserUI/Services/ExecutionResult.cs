namespace SetDefaultBrowserUI.Services;

public class ExecutionResult<T>
{
    private ExecutionResult(bool isSuccess, string error, T result)
    {
        IsSuccess = isSuccess;
        Error = error;
        Result = result;
    }

    public bool IsSuccess { get; }
    public T Result { get; }
    public string Error { get; }

    public static ExecutionResult<T> Success(T result)
    {
        return new ExecutionResult<T>(true, string.Empty, result);
    }

    public static ExecutionResult<T> Fail(string error)
    {
        return new ExecutionResult<T>(false, error, default!);
    }
}