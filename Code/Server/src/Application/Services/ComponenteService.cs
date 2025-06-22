using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class ComponenteService : IComponenteService
{
    private readonly IComponenteRepository _componenteRepository;
    public ComponenteService(IComponenteRepository componenteRepository) => _componenteRepository = componenteRepository;

    public void CrearComponente(CrearComponenteComando comando)
    {
        ValidarEntradaCreacion(comando);
        _componenteRepository.Crear(comando);
    }

    public List<ComponenteDto>? ObtenerTodosComponentes()
    {
        DataTable resultado = _componenteRepository.ObtenerTodos();
        var lista = new List<ComponenteDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    public void ActualizarComponente(ActualizarComponenteComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _componenteRepository.Actualizar(comando);
    }

    public void EliminarComponente(EliminarComponenteComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _componenteRepository.Eliminar(comando.Id);
    }

    private void ValidarEntradaCreacion(CrearComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (string.IsNullOrWhiteSpace(comando.Modelo)) throw new ErrorModeloRequerido();
        if (comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.CodigoIMT <= 0) throw new ErrorCodigoImtRequerido();
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }
    private void ValidarEntradaActualizacion(ActualizarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }
    private void ValidarEntradaEliminacion(EliminarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static ComponenteDto MapearFilaADto(DataRow fila) => new ComponenteDto
    {
        Id = Convert.ToInt32(fila["id_componente"]),
        Nombre = fila["nombre_componente"] == DBNull.Value ? null : fila["nombre_componente"].ToString(),
        Modelo = fila["modelo_componente"] == DBNull.Value ? null : fila["modelo_componente"].ToString(),
        Tipo = fila["tipo_componente"] == DBNull.Value ? null : fila["tipo_componente"].ToString(),
        Descripcion = fila["descripcion_componente"] == DBNull.Value ? null : fila["descripcion_componente"].ToString(),
        PrecioReferencia = fila["precio_referencia_componente"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_referencia_componente"]),
        NombreEquipo = fila["nombre_equipo"] == DBNull.Value ? null : fila["nombre_equipo"].ToString(),
        CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
        UrlDataSheet = fila["url_data_sheet_equipo"] == DBNull.Value ? null : fila["url_data_sheet_equipo"].ToString(),
    };
}