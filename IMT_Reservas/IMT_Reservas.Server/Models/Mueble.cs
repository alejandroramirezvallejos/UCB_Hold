public class Mueble
{
    public int Id_Mueble { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Ubicacion { get; set; }
    public float Dimensiones { get; set; }
    public int Numero_Gaveteros { get; set; }
    public float Costo {  get; set; }
    public int Id_Gavetero { get; set; }
    public Boolean Estado_Eliminado { get; set; }
}