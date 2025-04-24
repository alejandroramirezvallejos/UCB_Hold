public class Mantenimiento
{
    public int Id_Mantenimiento { get; set; }
    public string Tipo_Mantenimiento { get; set; }
    public string Descripcion { get; set; }
    public string Celular_Responsable { get; set; }
    public string Detalle { get; set; }
    public float Costo { get; set; }
    public DateOnly Fecha_Mantenimiento { get; set; }
    public int Id_Equipo { get; set; }
    public int Id_Empresa { get; set; }
    public Boolean Estado_Eliminado { get; set; }
    public string Apellido_Responsable { get; set; }
    public string Nombre_Responsable { get; set; }
    
    
    
    
}