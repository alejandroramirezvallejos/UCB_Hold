
public class ErrorDataBase : Exception
{
    public string? SqlState { get; }
    public string? Detail { get; }

    public ErrorDataBase(string message) : base(message)
    {
    }

    public ErrorDataBase(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ErrorDataBase(string message, string? sqlState, string? detail) : base(message)
    {
        SqlState = sqlState;
        Detail = detail;
    }

    public ErrorDataBase(string message, string? sqlState, string? detail, Exception innerException) 
        : base(message, innerException)
    {
        SqlState = sqlState;
        Detail = detail;
    }
}
