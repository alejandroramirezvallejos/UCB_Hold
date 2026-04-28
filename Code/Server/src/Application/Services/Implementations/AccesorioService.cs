using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class AccesorioService : BaseServicios,
    ICrearServicio<CrearAccesorioComando>,
    IActualizarServicio<ActualizarAccesorioComando>,
    IEliminarServicio<EliminarAccesorioComando>,
    IObtenerTodosServicio<AccesorioDto>
{
    private readonly AccesorioRepository _accesorioRepository;

    public AccesorioService(AccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }

    public virtual void Crear(CrearAccesorioComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: código IMT → id_equipo
        var idEquipo = _accesorioRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
        if (idEquipo == null)
            throw new ErrorCodigoImtNoEncontrado();

        _accesorioRepository.Crear(idEquipo.Value, comando);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        if (comando is CrearAccesorioComando accesorioComando)
        {
            if (string.IsNullOrWhiteSpace(accesorioComando.Nombre)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(accesorioComando.Modelo)) throw new ErrorModeloRequerido();
            if (accesorioComando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre del accesorio", 255);
            if (accesorioComando.CodigoIMT <= 0) throw new ErrorCodigoImtInvalido();
            if (accesorioComando.Precio.HasValue && accesorioComando.Precio.Value <= 0) throw new ErrorValorNegativo("precio");
        }
    }
    public virtual List<AccesorioDto>? ObtenerTodos()
    {
        try
        {
            DataTable dt = _accesorioRepository.ObtenerTodos();
            var lista = new List<AccesorioDto>(dt.Rows.Count);            
            foreach (DataRow row in dt.Rows)
            {
                var baseDto = MapearFilaADto(row);
                if (baseDto is AccesorioDto accesorio)
                    lista.Add(accesorio);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void Actualizar(ActualizarAccesorioComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el accesorio exista y esté activo
        if (!_accesorioRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Resolver FK: código IMT → id_equipo (si se proporcionó)
        int? idEquipo = null;
        if (comando.CodigoIMT > 0)
        {
            idEquipo = _accesorioRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
            if (idEquipo == null)
                throw new ErrorCodigoImtNoEncontrado();
        }

        _accesorioRepository.Actualizar(idEquipo, comando);
    }
    private void ValidarEntradaActualizacion(ActualizarAccesorioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("accesorio");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre del accesorio", 255);
        if (comando.CodigoIMT <= 0) throw new ErrorCodigoImtInvalido();
        if (comando.Precio.HasValue && comando.Precio.Value < 0) throw new ErrorValorNegativo("precio");
    }
    public virtual void Eliminar(EliminarAccesorioComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el accesorio exista y esté activo
        if (!_accesorioRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _accesorioRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        if (comando is EliminarAccesorioComando accesorioComando)
        {
            if (accesorioComando.Id <= 0) throw new ErrorIdInvalido("accesorio");
        }
    }    
    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id = Convert.ToInt32(fila["id_accesorio"]),
            Nombre = fila["nombre_accesorio"] == DBNull.Value ? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
            UrlDataSheet = fila["url_data_sheet_accesorio"] == DBNull.Value ? null : fila["url_data_sheet_accesorio"].ToString(),
        };
    }
}