public class Mueble
{
    public int      Id              { get; private set; }
    public string   Nombre          { get; private set; }
    public string   Tipo            { get; private set; }
    public string   Ubicacion       { get; private set; }
    public string   Dimensiones     { get; private set; }
    public int      NumeroGaveteros { get; private set; }
    public decimal  Costo           { get; private set; }
    public int      GaveteroId      { get; private set; }
    public Gavetero Gavetero        { get; private set; }
    public bool     EstaEliminado   { get; private set; }

    public Mueble(string nombre, string tipo, string ubicacion, string dimensiones,
                  int numeroGaveteros, decimal costo, int gaveteroId)
    {
        Nombre           = nombre;
        Tipo             = tipo;
        Ubicacion        = ubicacion;
        Dimensiones      = dimensiones;
        NumeroGaveteros  = numeroGaveteros;
        Costo            = costo;
        GaveteroId       = gaveteroId;
        EstaEliminado    = false;
    }
}
