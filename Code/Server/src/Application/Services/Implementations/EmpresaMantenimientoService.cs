using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class EmpresaMantenimientoService : BaseServicios,
    ICrearServicio<CrearEmpresaMantenimientoComando>,
    IActualizarServicio<ActualizarEmpresaMantenimientoComando>,
    IEliminarServicio<EliminarEmpresaMantenimientoComando>,
    IObtenerTodosServicio<EmpresaMantenimientoDto>
{
    private readonly EmpresaMantenimientoRepository _empresaRepository;

    public EmpresaMantenimientoService(EmpresaMantenimientoRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    public virtual void Crear(CrearEmpresaMantenimientoComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Intentar reactivar si existe una empresa eliminada lógicamente con ese nombre
        if (_empresaRepository.ReactivarEliminadaPorNombre(comando.NombreEmpresa!))
            return;

        // Verificar si ya existe una empresa activa con ese nombre
        if (_empresaRepository.ExisteActivaPorNombre(comando.NombreEmpresa!))
            throw new ErrorRegistroYaExiste();

        _empresaRepository.Crear(comando);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearEmpresaMantenimientoComando
        if (comando is CrearEmpresaMantenimientoComando empresaComando)
        {
            if (string.IsNullOrWhiteSpace(empresaComando.NombreEmpresa)) throw new ErrorNombreRequerido();
            if (empresaComando.NombreEmpresa.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
            if (!string.IsNullOrWhiteSpace(empresaComando.Telefono) && empresaComando.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
        }
    }

    public virtual List<EmpresaMantenimientoDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _empresaRepository.ObtenerTodos();
            var lista = new List<EmpresaMantenimientoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as EmpresaMantenimientoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    
    public virtual void Actualizar(ActualizarEmpresaMantenimientoComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que la empresa exista y esté activa
        if (!_empresaRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Verificar duplicados si se está cambiando el nombre
        if (!string.IsNullOrWhiteSpace(comando.NombreEmpresa))
        {
            if (_empresaRepository.ExisteActivaPorNombreExcluyendoId(comando.NombreEmpresa, comando.Id))
                throw new ErrorRegistroYaExiste();

            // Reactivar si existe eliminada lógicamente con ese nombre (como en el SP)
            if (_empresaRepository.ReactivarEliminadaPorNombre(comando.NombreEmpresa))
            {
                _empresaRepository.EliminarLogicamentePorId(comando.Id);
                return;
            }
        }

        _empresaRepository.Actualizar(comando);
    }

    private void ValidarEntradaActualizacion(ActualizarEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("empresa de mantenimiento");
        if (!string.IsNullOrWhiteSpace(comando.NombreEmpresa) && comando.NombreEmpresa.Length > 255)
            throw new ErrorLongitudInvalida("nombre de la empresa", 255);
    }

    public virtual void Eliminar(EliminarEmpresaMantenimientoComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que la empresa exista y esté activa
        if (!_empresaRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _empresaRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarEmpresaMantenimientoComando
        if (comando is EliminarEmpresaMantenimientoComando empresaComando)
        {
            if (empresaComando.Id <= 0) throw new ErrorIdInvalido("empresa de mantenimiento");
        }
    }

    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new EmpresaMantenimientoDto
        {
            Id = Convert.ToInt32(fila["id_empresa_mantenimiento"]),
            NombreEmpresa = fila["nombre_empresa"] == DBNull.Value ? null : fila["nombre_empresa"].ToString(),
            NombreResponsable = fila["nombre_responsable_empresa"] == DBNull.Value ? null : fila["nombre_responsable_empresa"].ToString(),
            ApellidoResponsable = fila["apellido_responsable_empresa"] == DBNull.Value ? null : fila["apellido_responsable_empresa"].ToString(),
            Telefono = fila["telefono_empresa"] == DBNull.Value ? null : fila["telefono_empresa"].ToString(),
            Direccion = fila["direccion_empresa"] == DBNull.Value ? null : fila["direccion_empresa"].ToString(),
            Nit = fila["nit_empresa"] == DBNull.Value ? null : fila["nit_empresa"].ToString()
        };
    }
}