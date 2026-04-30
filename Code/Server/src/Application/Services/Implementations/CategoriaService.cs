using System.Data;
using Ardalis.Result;

public class CategoriaService : BaseServicios, ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public virtual Result<CategoriaDto> Crear(CrearCategoriaComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CategoriaDto>.Invalid(validResult.ValidationErrors.ToArray());

        var nombreTrimmed = comando.Nombre!.Trim();

        if (string.IsNullOrWhiteSpace(nombreTrimmed))
            return Result<CategoriaDto>.Invalid(new ValidationError("Nombre", "El nombre es requerido"));

        if (_categoriaRepository.ReactivarEliminadaPorNombre(nombreTrimmed))
            return Result<CategoriaDto>.Success(null);

        if (_categoriaRepository.ExisteActivaPorNombre(nombreTrimmed))
            return Result<CategoriaDto>.Conflict("Ya existe una categoría activa con este nombre");

        var comandoFinal = new CrearCategoriaComando(nombreTrimmed);
        var result = _categoriaRepository.Crear(comandoFinal);
        return result;
    }

    public virtual Result<List<CategoriaDto>> ObtenerTodos()
    {
        var repoResult = _categoriaRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<CategoriaDto>>.Error("Error al obtener las categorías");

        var resultado = repoResult.Value;
        var lista = new List<CategoriaDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var baseDto = MapearFilaADto(fila);
            if (baseDto is CategoriaDto categoria)
                lista.Add(categoria);
        }
        return lista.Count == 0
            ? Result<List<CategoriaDto>>.NotFound("No se encontraron categorías")
            : Result<List<CategoriaDto>>.Success(lista);
    }

    public virtual Result<CategoriaDto> Actualizar(ActualizarCategoriaComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CategoriaDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_categoriaRepository.ExisteActivaPorId(comando.Id))
            return Result<CategoriaDto>.NotFound("La categoría no fue encontrada");

        var nombreNuevo = comando.Nombre?.Trim();

        if (nombreNuevo != null)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevo))
                return Result<CategoriaDto>.Invalid(new ValidationError("Nombre", "El nombre es requerido"));

            if (_categoriaRepository.ExisteActivaPorNombreExcluyendoId(nombreNuevo, comando.Id))
                return Result<CategoriaDto>.Conflict("Ya existe otra categoría activa con ese nombre");

            if (_categoriaRepository.ReactivarEliminadaPorNombre(nombreNuevo))
            {
                _categoriaRepository.EliminarLogicamentePorId(comando.Id);
                return Result<CategoriaDto>.Success(null);
            }
        }

        var comandoFinal = new ActualizarCategoriaComando(comando.Id, nombreNuevo);
        var result = _categoriaRepository.Actualizar(comandoFinal);
        return result;
    }

    public virtual Result<CategoriaDto> Eliminar(EliminarCategoriaComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CategoriaDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_categoriaRepository.ExisteActivaPorId(comando.Id))
            return Result<CategoriaDto>.NotFound("La categoría no fue encontrada");

        var result = _categoriaRepository.Eliminar(comando);
        return result;
    }

    private Result<CrearCategoriaComando> ValidarEntrada(CrearCategoriaComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Nombre?.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        return errors.Any()
            ? Result<CrearCategoriaComando>.Invalid(errors.ToArray())
            : Result<CrearCategoriaComando>.Success(comando!);
    }

    private Result<ActualizarCategoriaComando> ValidarEntrada(ActualizarCategoriaComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        return errors.Any()
            ? Result<ActualizarCategoriaComando>.Invalid(errors.ToArray())
            : Result<ActualizarCategoriaComando>.Success(comando!);
    }

    private Result<EliminarCategoriaComando> ValidarEntrada(EliminarCategoriaComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarCategoriaComando>.Invalid(errors.ToArray())
            : Result<EliminarCategoriaComando>.Success(comando!);
    }

    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new CategoriaDto
        {
            Id = Convert.ToInt32(fila["id_categoria"]),
            Nombre = fila["categoria"] == DBNull.Value ? null : fila["categoria"].ToString(),
        };
    }
}
