public class Componente
{
    public int     Id               { get; private set; }
    public string  Nombre           { get; private set; }
    public string  Descripcion      { get; private set; }
    public string  Modelo           { get; private set; }
    public Uri     DataSheetUrl     { get; private set; }
    public string  Tipo             { get; private set; }
    public decimal PrecioReferencia { get; private set; }
    public int     EquipoId         { get; private set; }
    public Equipo  Equipo           { get; private set; }
    public bool    EstaEliminado    { get; private set; }

    public Componente(string nombre, string descripcion, string modelo, Uri dataSheetUrl, 
                      string tipo, decimal precioReferencia, int equipoId)
    {
        Nombre            = nombre;
        Descripcion       = descripcion;
        Modelo            = modelo;
        DataSheetUrl      = dataSheetUrl;
        Tipo              = tipo;
        PrecioReferencia  = precioReferencia;
        EquipoId          = equipoId;
        EstaEliminado     = false;
    }
}
