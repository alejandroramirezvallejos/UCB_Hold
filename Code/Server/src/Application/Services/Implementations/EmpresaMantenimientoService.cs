using System.Data;
using Ardalis.Result;

public class EmpresaMantenimientoService : Service<EmpresaMantenimientoDto>, ICrud<EmpresaMantenimientoDto, CrearEmpresaMantenimientoComando, ActualizarEmpresaMantenimientoComando, EliminarEmpresaMantenimientoComando>
{
    private readonly IEmpresaMantenimientoRepository _empresaRepository;

    public EmpresaMantenimientoService(IEmpresaMantenimientoRepository empresaRepository) => _empresaRepository = empresaRepository;

    public Result<EmpresaMantenimientoDto?> Crear(CrearEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (_empresaRepository.ReactivarEliminadaPorNombre(comando.NombreEmpresa!))
            return Result<EmpresaMantenimientoDto?>.Success(null);

        if (_empresaRepository.ExisteActivaPorNombre(comando.NombreEmpresa!))
            return Result<EmpresaMantenimientoDto?>.Conflict("Ya existe una empresa de mantenimiento activa con este nombre");

        return _empresaRepository.Crear(comando);
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _empresaRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener las empresas de mantenimiento");
        return result;
    }

    public Result<EmpresaMantenimientoDto?> Actualizar(ActualizarEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_empresaRepository.ExisteActivaPorId(comando.Id))
            return Result<EmpresaMantenimientoDto?>.NotFound("La empresa de mantenimiento no fue encontrada");

        if (!string.IsNullOrWhiteSpace(comando.NombreEmpresa))
        {
            if (_empresaRepository.ExisteActivaPorNombreExcluyendoId(comando.NombreEmpresa, comando.Id))
                return Result<EmpresaMantenimientoDto?>.Conflict("Ya existe otra empresa activa con ese nombre");

            if (_empresaRepository.ReactivarEliminadaPorNombre(comando.NombreEmpresa))
            {
                _empresaRepository.EliminarLogicamentePorId(comando.Id);
                return Result<EmpresaMantenimientoDto?>.Success(null);
            }
        }

        return _empresaRepository.Actualizar(comando);
    }

    public Result<EmpresaMantenimientoDto?> Eliminar(EliminarEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_empresaRepository.ExisteActivaPorId(comando.Id))
            return Result<EmpresaMantenimientoDto?>.NotFound("La empresa de mantenimiento no fue encontrada");

        return _empresaRepository.Eliminar(comando);
    }

    private Result<CrearEmpresaMantenimientoComando> ValidarEntrada(CrearEmpresaMantenimientoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.NombreEmpresa))
            errors.Add(new("NombreEmpresa", "El nombre de la empresa es requerido"));

        if (comando?.NombreEmpresa?.Length > 255)
            errors.Add(new("NombreEmpresa", "El nombre no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Telefono) && comando.Telefono.Length > 20)
            errors.Add(new("Telefono", "El teléfono no puede tener más de 20 caracteres"));

        return errors.Any()
            ? Result<CrearEmpresaMantenimientoComando>.Invalid(errors.ToArray())
            : Result<CrearEmpresaMantenimientoComando>.Success(comando!);
    }

    private Result<ActualizarEmpresaMantenimientoComando> ValidarEntrada(ActualizarEmpresaMantenimientoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.NombreEmpresa) && comando.NombreEmpresa.Length > 255)
            errors.Add(new("NombreEmpresa", "El nombre de la empresa no puede tener más de 255 caracteres"));

        return errors.Any()
            ? Result<ActualizarEmpresaMantenimientoComando>.Invalid(errors.ToArray())
            : Result<ActualizarEmpresaMantenimientoComando>.Success(comando!);
    }

    private Result<EliminarEmpresaMantenimientoComando> ValidarEntrada(EliminarEmpresaMantenimientoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarEmpresaMantenimientoComando>.Invalid(errors.ToArray())
            : Result<EliminarEmpresaMantenimientoComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila) => new EmpresaMantenimientoDto
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
