namespace IMT_Reservas.Server.Application.Common;

public class Response<T> where T : class
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    public string? Message { get; set; }
}
