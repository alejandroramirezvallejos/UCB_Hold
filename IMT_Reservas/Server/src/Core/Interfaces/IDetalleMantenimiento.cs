public interface IDetalleMantenimiento
{
    int     Id              { get; }
    int     IdMantenimiento { get; }
    string? Descripcion     { get; }
    int     IdEquipo        { get; }
}
