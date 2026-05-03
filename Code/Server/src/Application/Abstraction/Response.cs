namespace IMT_Reservas.Server.Application.Abstraction;

public class Response<T> where T : class
{
    public int Status { get; set; }
    public T? Value { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<ValidationError> ValidationErrors { get; set; } = [];
    public string? SuccessMessage { get; set; }
}
