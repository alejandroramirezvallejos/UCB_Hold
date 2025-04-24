public enum AccesorioTipo { Estandar }

public class Accesorio
{
    public int           Id            { get; private set; }
    public string        Nombre        { get; private set; }
    public string        Descripcion   { get; private set; }
    public string        Modelo        { get; private set; }
    public Uri           DataSheetUrl  { get; private set; }
    public decimal       Precio        { get; private set; }
    public int           EquipoId      { get; private set; }
    public Equipo        Equipo        { get; private set; } 
    public AccesorioTipo Tipo          { get; private set; }
    public bool          EstaEliminado { get; private set; }

    public Accesorio(string nombre, string descripcion, string modelo, Uri dataSheetUrl,
                     decimal precio, int equipoId, AccesorioTipo tipo)
    {
        Nombre        = nombre;
        Descripcion   = descripcion;
        Modelo        = modelo;
        DataSheetUrl  = dataSheetUrl;
        Precio        = precio;
        EquipoId      = equipoId;
        Tipo          = tipo;
        EstaEliminado = false;
    }

    public void Eliminar() => EstaEliminado = true;
}
