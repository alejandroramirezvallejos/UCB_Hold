namespace IMT_Reservas.Server.Application.Abstraction;

public abstract class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
