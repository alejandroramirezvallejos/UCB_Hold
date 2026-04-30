using System.Data;
using Ardalis.Result;

public class MuebleService : Service
{
    private readonly IMuebleRepository _muebleRepository;

    public MuebleService(IMuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }

    public virtual Result<MuebleDto?> Crear(CrearMuebleComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<MuebleDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var result = _muebleRepository.Crear(comando);
        return result;
    }

    public virtual Result<List<MuebleDto?>> ObtenerTodos()
    {
        var repoResult = _muebleRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<MuebleDto?>>.Error("Error al obtener los muebles");

        var resultado = repoResult.Value;
        var lista = new List<MuebleDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as MuebleDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<MuebleDto?>>.NotFound("No se encontraron muebles")
            : Result<List<MuebleDto?>>.Success(lista);
    }

    public virtual Result<MuebleDto?> Actualizar(ActualizarMuebleComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<MuebleDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_muebleRepository.ExisteActivoPorId(comando.Id))
            return Result<MuebleDto?>.NotFound("El mueble no fue encontrado");

        var result = _muebleRepository.Actualizar(comando);
        return result;
    }

    public virtual Result<MuebleDto?> Eliminar(EliminarMuebleComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<MuebleDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_muebleRepository.ExisteActivoPorId(comando.Id))
            return Result<MuebleDto?>.NotFound("El mueble no fue encontrado");

        var result = _muebleRepository.Eliminar(comando);
        return result;
    }

    private Result<CrearMuebleComando> ValidarEntrada(CrearMuebleComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Costo.HasValue == true && comando.Costo < 0)
            errors.Add(new("Costo", "El costo no puede ser negativo"));

        if (comando?.Longitud.HasValue == true && comando.Longitud <= 0)
            errors.Add(new("Longitud", "La longitud debe ser mayor a 0"));

        if (comando?.Profundidad.HasValue == true && comando.Profundidad <= 0)
            errors.Add(new("Profundidad", "La profundidad debe ser mayor a 0"));

        if (comando?.Altura.HasValue == true && comando.Altura <= 0)
            errors.Add(new("Altura", "La altura debe ser mayor a 0"));

        return errors.Any()
            ? Result<CrearMuebleComando>.Invalid(errors.ToArray())
            : Result<CrearMuebleComando>.Success(comando!);
    }

    private Result<ActualizarMuebleComando> ValidarEntrada(ActualizarMuebleComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (comando?.Costo.HasValue == true && comando.Costo < 0)
            errors.Add(new("Costo", "El costo no puede ser negativo"));

        if (comando?.Longitud.HasValue == true && comando.Longitud <= 0)
            errors.Add(new("Longitud", "La longitud debe ser mayor a 0"));

        if (comando?.Profundidad.HasValue == true && comando.Profundidad <= 0)
            errors.Add(new("Profundidad", "La profundidad debe ser mayor a 0"));

        if (comando?.Altura.HasValue == true && comando.Altura <= 0)
            errors.Add(new("Altura", "La altura debe ser mayor a 0"));

        return errors.Any()
            ? Result<ActualizarMuebleComando>.Invalid(errors.ToArray())
            : Result<ActualizarMuebleComando>.Success(comando!);
    }

    private Result<EliminarMuebleComando> ValidarEntrada(EliminarMuebleComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarMuebleComando>.Invalid(errors.ToArray())
            : Result<EliminarMuebleComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila)
    {
        return new MuebleDto
        {
            Id = Convert.ToInt32(fila["id_mueble"]),
            Nombre = fila["nombre_mueble"] == DBNull.Value ? null : fila["nombre_mueble"].ToString(),
            NumeroGaveteros = fila["numero_gaveteros_mueble"] == DBNull.Value ? null : Convert.ToInt32(fila["numero_gaveteros_mueble"]),
            Ubicacion = fila["ubicacion_mueble"] == DBNull.Value ? null : fila["ubicacion_mueble"].ToString(),
            Tipo = fila["tipo_mueble"] == DBNull.Value ? null : fila["tipo_mueble"].ToString(),
            Costo = fila["costo_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mueble"]),
            Longitud = fila["longitud_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_mueble"]),
            Profundidad = fila["profundidad_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_mueble"]),
            Altura = fila["altura_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_mueble"])
        };
    }
}
