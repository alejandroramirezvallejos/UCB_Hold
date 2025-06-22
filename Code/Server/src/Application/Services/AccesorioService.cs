using System.Data;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

public class AccesorioService : IAccesorioService
{
    private readonly IAccesorioRepository _accesorioRepository;
    public AccesorioService(IAccesorioRepository accesorioRepository) => _accesorioRepository = accesorioRepository;

    public void CrearAccesorio(CrearAccesorioComando comando)
    {
        ValidarEntradaCreacion(comando);
        _accesorioRepository.Crear(comando);
    }

    public List<AccesorioDto>? ObtenerTodosAccesorios()
    {
        DataTable dt = _accesorioRepository.ObtenerTodos();
        var lista = new List<AccesorioDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
    }

    public void ActualizarAccesorio(ActualizarAccesorioComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _accesorioRepository.Actualizar(comando);
    }

    public void EliminarAccesorio(EliminarAccesorioComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _accesorioRepository.Eliminar(comando.Id);
    }

    private static AccesorioDto MapearFilaADto(DataRow fila) => new AccesorioDto
    {
        Id = Convert.ToInt32(fila["id_accesorio"]),
        Nombre = fila["nombre_accesorio"] == DBNull.Value ? null : fila["nombre_accesorio"].ToString(),
        Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
        Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
        Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
        NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
        CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
        Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
        UrlDataSheet = fila["url_data_sheet_accesorio"] == DBNull.Value ? null : fila["url_data_sheet_accesorio"].ToString(),
    };

    private void ValidarEntradaCreacion(CrearAccesorioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (string.IsNullOrWhiteSpace(comando.Modelo)) throw new ErrorModeloRequerido();
        if (comando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre del accesorio", 256);
        if (comando.CodigoIMT <= 0) throw new ErrorIdInvalido();
        if (comando.Precio.HasValue && comando.Precio.Value <= 0) throw new ErrorValorNegativo("precio");
    }
    private void ValidarEntradaActualizacion(ActualizarAccesorioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre del accesorio", 255);
        if (comando.CodigoIMT <= 0) throw new ErrorIdInvalido();
        if (comando.Precio.HasValue && comando.Precio.Value < 0) throw new ErrorValorNegativo("precio");
    }
    private void ValidarEntradaEliminacion(EliminarAccesorioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
}