namespace IMT_Reservas.Server.Core.Abstraction;

public abstract class QueryFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Dictionary<string, object>? Filters { get; set; }
}
