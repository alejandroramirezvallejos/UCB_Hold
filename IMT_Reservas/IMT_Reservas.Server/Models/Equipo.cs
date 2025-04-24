public class Equipo
{
    public string Id_Equipo { get; set; }
    public string Id_Grupo_Equipo { get; set; }
    public string Codigo_IMT { get; set; }
    public string Codigo_UCB { get; set; }
    public string Descripcion { get; set; }
    public string Estado_Equipo { get; set; }
    public string Numero_Serial { get; set; }
    public string Ubicacion { get; set; }
    public decimal? Costo_Referencia { get; set; }
    public int? Tiempo_Max_Prestamo { get; set; }
    public string Procedencia { get; set; }
    public int Id_Gavetero {  get; set; }
    public Boolean Estado_Eliminado { get; set; }
    public string Estado_Disponibilidad { get; set; }
}