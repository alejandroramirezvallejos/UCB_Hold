namespace IMT_Reservas.Server.Application.Features.Cache;

public static class CacheKeys
{
    public static string GrupoEquipoSearch(string nombre, string? categoria)
        => $"grupo-equipo:search:{nombre}:{categoria}";

    public static string Usuario(string carnet) => $"usuario:{carnet}";
}
