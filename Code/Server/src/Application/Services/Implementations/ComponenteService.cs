using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class ComponenteService : BaseServicios,
    ICrearServicio<CrearComponenteComando>,
    IActualizarServicio<ActualizarComponenteComando>,
    IEliminarServicio<EliminarComponenteComando>,
    IObtenerTodosServicio<ComponenteDto>
{
    private readonly ComponenteRepository _componenteRepository;

    public ComponenteService(ComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }

    public virtual void Crear(CrearComponenteComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: código IMT → id_equipo
        var idEquipo = _componenteRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
        if (idEquipo == null)
            throw new ErrorCodigoImtNoEncontrado();

        _componenteRepository.Crear(idEquipo.Value, comando);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearComponenteComando
        if (comando is CrearComponenteComando componenteComando)
        {
            if (string.IsNullOrWhiteSpace(componenteComando.Nombre)) throw new ErrorNombreRequerido();
            if (componenteComando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
            if(string.IsNullOrWhiteSpace(componenteComando.Modelo)) throw new ErrorModeloRequerido();
            if (componenteComando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
            if (componenteComando.CodigoIMT <= 0) throw new ErrorCodigoImtRequerido();
            if (componenteComando.PrecioReferencia.HasValue && componenteComando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
        }
    }
    
    public virtual List<ComponenteDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _componenteRepository.ObtenerTodos();
            var lista = new List<ComponenteDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as ComponenteDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void Actualizar(ActualizarComponenteComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el componente exista y esté activo
        if (!_componenteRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Resolver FK: código IMT → id_equipo (si se proporcionó)
        int? idEquipo = null;
        if (comando.CodigoIMT.HasValue && comando.CodigoIMT.Value > 0)
        {
            idEquipo = _componenteRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT.Value);
            if (idEquipo == null)
                throw new ErrorCodigoImtNoEncontrado();
        }

        _componenteRepository.Actualizar(idEquipo, comando);
    }

    private void ValidarEntradaActualizacion(ActualizarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("componente");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }

    public virtual void Eliminar(EliminarComponenteComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el componente exista y esté activo
        if (!_componenteRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _componenteRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarComponenteComando
        if (comando is EliminarComponenteComando componenteComando)
        {
            if (componenteComando.Id <= 0) throw new ErrorIdInvalido("componente");
        }
    }

    protected override BaseDto MapearFilaADto(DataRow fila) => new ComponenteDto
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