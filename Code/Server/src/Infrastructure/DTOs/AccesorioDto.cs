public class AccesorioDto //TODOS LOS DTOS SE PUSIERON IGUAL QUE LA DEVOLUCION DE SELECTS EN LA DB
{
    public string?  Nombre        { get; set; } = null;
    public string? Modelo        { get; set; } = null;
    public string? Tipo          { get; set; } = null;
    public double? Precio        { get; set; } = null;
    public string? NombreEquipoAsociado { get; set; } = null;
    public int? CodigoImtEquipoAsociado { get; set; } = null;
    public string? Descripcion   { get; set; } = null;
}