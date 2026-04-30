using System.Data;
using Ardalis.Result;

public class EmpresaMantenimientoService : Service
{
    private readonly IEmpresaMantenimientoRepository _empresaRepository;

    public EmpresaMantenimientoService(IEmpresaMantenimientoRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    public virtual Result<EmpresaMantenimientoDto?> Crear(CrearEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (_empresaRepository.ReactivarEliminadaPorNombre(comando.NombreEmpresa!))
            return Result<EmpresaMantenimientoDto?>.Success(null);

        if (_empresaRepository.ExisteActivaPorNombre(comando.NombreEmpresa!))
            return Result<EmpresaMantenimientoDto?>.Conflict("Ya existe una empresa de mantenimiento activa con este nombre");

        var result = _empresaRepository.Crear(comando);
        return result;
    }

    public virtual Result<List<EmpresaMantenimientoDto?>> ObtenerTodos()
    {
        var repoResult = _empresaRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<EmpresaMantenimientoDto?>>.Error("Error al obtener las empresas de mantenimiento");

        var resultado = repoResult.Value;
        var lista = new List<EmpresaMantenimientoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as EmpresaMantenimientoDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<EmpresaMantenimientoDto?>>.NotFound("No se encontraron empresas de mantenimiento")
            : Result<List<EmpresaMantenimientoDto?>>.Success(lista);
    }

    public virtual Result<EmpresaMantenimientoDto?> Actualizar(ActualizarEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

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

        var result = _empresaRepository.Actualizar(comando);
        return result;
    }

    public virtual Result<EmpresaMantenimientoDto?> Eliminar(EliminarEmpresaMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EmpresaMantenimientoDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_empresaRepository.ExisteActivaPorId(comando.Id))
            return Result<EmpresaMantenimientoDto?>.NotFound("La empresa de mantenimiento no fue encontrada");

        var result = _empresaRepository.Eliminar(comando);
        return result;
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

    protected override Dto MapearFilaADto(DataRow fila)
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
