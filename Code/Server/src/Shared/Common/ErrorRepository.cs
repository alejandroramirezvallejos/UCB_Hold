
public class ErrorRepository : Exception
{
    public string? Operation { get; }
    public string? Entity { get; }

    public ErrorRepository(string message) : base(message)
    {
    }

    public ErrorRepository(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ErrorRepository(string message, string? operation, string? entity) : base(message)
    {
        Operation = operation;
        Entity = entity;
    }

    public ErrorRepository(string message, string? operation, string? entity, Exception innerException) 
        : base(message, innerException)
    {
        Operation = operation;
        Entity = entity;
    }
}

