public class Prestamo
{
    public int Id_Prestamo { get; set; }
    public DateTime Fecha_Solicitud { get; set; }
    public DateTime Fecha_Prestamo { get; set; }
    public DateTime Fecha_Devolucion { get; set; }
    public string Observacion { get; set; }
    public string Estado_Prestamo { get; set; }
    public string Carnet {  get; set; }
    public int Id_Equipo { get; set; }
    public Boolean Estado_Eliminado { get; set; }
}