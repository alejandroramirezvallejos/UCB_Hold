public enum EstadoDisponibilidad { Disponible, Reservado, Revision, Mantenimiento }
public enum EstadoEquipo         { Inoperativo, Parcialmente_Operativo, Operativo }

public class Equipo
{
    public int                  Id                   { get; private set; }
    public int                  GrupoEquipoId        { get; private set; }
    public GrupoEquipo          GrupoEquipo          { get; private set; }
    public string               CodigoImt            { get; private set; }
    public string               CodigoUcb            { get; private set; }
    public string               Descripcion          { get; private set; }
    public EstadoEquipo         EstadoEquipo         { get; private set; }
    public string               NumeroSerial         { get; private set; }
    public string               Ubicacion            { get; private set; }
    public double?              CostoReferencia      { get; private set; }
    public int?                 TiempoMaximoPrestamo { get; private set; }
    public string               Procedencia          { get; private set; }
    public int                  GaveteroId           { get; private set; }
    public Gavetero             Gavetero             { get; private set; }
    public bool                 EstaEliminado        { get; private set; }
    public EstadoDisponibilidad EstadoDisponibilidad { get; private set; }

    public Equipo(int grupoEquipoId, string codigoImt, string codigoUcb, string descripcion, 
                  EstadoEquipo estadoEquipo, string numeroSerial, string ubicacion, 
                  double? costoReferencia, int? tiempoMaximoPrestamo, string procedencia, 
                  int gaveteroId, EstadoDisponibilidad estadoDisponibilidad)
    {
        GrupoEquipoId        = grupoEquipoId;
        CodigoImt            = codigoImt;
        CodigoUcb            = codigoUcb;
        Descripcion          = descripcion;
        EstadoEquipo         = estadoEquipo;
        NumeroSerial         = numeroSerial;
        Ubicacion            = ubicacion;
        CostoReferencia      = costoReferencia;
        TiempoMaximoPrestamo = tiempoMaximoPrestamo;
        Procedencia          = procedencia;
        GaveteroId           = gaveteroId;
        EstadoDisponibilidad = estadoDisponibilidad;
        EstaEliminado        = false;
    }
}
