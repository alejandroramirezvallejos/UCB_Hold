using Microsoft.AspNetCore.Http.Timeouts;

public class Componente
{
    public int Id_Componente { get; set; }
    public string Descripcion { get; set; }
    public string Modelo { get; set; }
    public string URL_Data_Sheet { get; set; }
    public string Tipo {  get; set; }
    public float Precio_Referencia { get; set; }
    public string Nombre_Componente { get; set; }
    public int Id_Equipo { get; set; }
    public Boolean Estado_Eliminado { get; set; }
}