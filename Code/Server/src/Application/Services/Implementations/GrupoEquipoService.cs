using System.Data;
using Ardalis.Result;

public class GrupoEquipoService : BaseServicios, IGrupoEquipoService
{
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;
    public GrupoEquipoService(IGrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }

    public virtual Result<GrupoEquipoDto> Crear(CrearGrupoEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<GrupoEquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        var idCategoria = _grupoEquipoRepository.ObtenerCategoriaIdPorNombre(comando.NombreCategoria!);
        if (idCategoria == null)
            return Result<GrupoEquipoDto>.NotFound("La categoría no fue encontrada");

        if (_grupoEquipoRepository.ExisteDuplicadoPorNombreModeloMarca(comando.Nombre!, comando.Modelo!, comando.Marca!))
            return Result<GrupoEquipoDto>.Conflict("Ya existe un grupo de equipo con estos datos");

        var result = _grupoEquipoRepository.Crear(idCategoria.Value, comando);
        return result;
    }

    public virtual Result<List<GrupoEquipoDto>> ObtenerTodos()
    {
        var repoResult = _grupoEquipoRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<GrupoEquipoDto>>.Error("Error al obtener los grupos de equipos");

        var resultado = repoResult.Value;
        var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as GrupoEquipoDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<GrupoEquipoDto>>.NotFound("No se encontraron grupos de equipos")
            : Result<List<GrupoEquipoDto>>.Success(lista);
    }

    public virtual Result<GrupoEquipoDto> ActualizarResult(ActualizarGrupoEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<GrupoEquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_grupoEquipoRepository.ExisteActivoPorId(comando.Id))
            return Result<GrupoEquipoDto>.NotFound("El grupo de equipo no fue encontrado");

        int? idCategoria = null;

        if (!string.IsNullOrWhiteSpace(comando.NombreCategoria))
        {
            idCategoria = _grupoEquipoRepository.ObtenerCategoriaIdPorNombre(comando.NombreCategoria);
            if (idCategoria == null)
                return Result<GrupoEquipoDto>.NotFound("La categoría no fue encontrada");
        }

        if (!string.IsNullOrWhiteSpace(comando.Nombre) || !string.IsNullOrWhiteSpace(comando.Modelo) || !string.IsNullOrWhiteSpace(comando.Marca))
        {
            var actual = _grupoEquipoRepository.ObtenerPorId(comando.Id);
            if (actual != null && actual.Rows.Count > 0)
            {
                var nombreFinal = !string.IsNullOrWhiteSpace(comando.Nombre) ? comando.Nombre : actual.Rows[0]["nombre_grupo_equipo"]?.ToString();
                var modeloFinal = !string.IsNullOrWhiteSpace(comando.Modelo) ? comando.Modelo : actual.Rows[0]["modelo_grupo_equipo"]?.ToString();
                var marcaFinal = !string.IsNullOrWhiteSpace(comando.Marca) ? comando.Marca : actual.Rows[0]["marca_grupo_equipo"]?.ToString();

                if (nombreFinal != null && modeloFinal != null && marcaFinal != null)
                {
                    if (_grupoEquipoRepository.ExisteDuplicadoPorNombreModeloMarcaExcluyendoId(nombreFinal, modeloFinal, marcaFinal, comando.Id))
                        return Result<GrupoEquipoDto>.Conflict("Ya existe otro grupo con estos datos");
                }
            }
        }

        var result = _grupoEquipoRepository.Actualizar(idCategoria, comando);
        return result;
    }

    public virtual Result<GrupoEquipoDto> Actualizar(ActualizarGrupoEquipoComando comando)
    {
        return ActualizarResult(comando);
    }

    public virtual Result<GrupoEquipoDto> Eliminar(EliminarGrupoEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<GrupoEquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_grupoEquipoRepository.ExisteActivoPorId(comando.Id))
            return Result<GrupoEquipoDto>.NotFound("El grupo de equipo no fue encontrado");

        var result = _grupoEquipoRepository.Eliminar(comando);
        return result;
    }

    public virtual GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        try
        {
            if (consulta == null) throw new ArgumentNullException(nameof(consulta), "La consulta es requerida");
            if (consulta.Id <= 0) throw new ArgumentException("El ID debe ser mayor a 0", nameof(consulta.Id));
            DataTable? resultado = _grupoEquipoRepository.ObtenerPorId(consulta.Id);
            if (resultado?.Rows.Count > 0)
            {
                var dto = MapearFilaADto(resultado.Rows[0]) as GrupoEquipoDto;
                return dto;
            }
            return null;
        }
        catch { throw; }
    }

    public virtual List<GrupoEquipoDto>? ObtenerGrupoEquipoPorNombreYCategoria(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta)
    {
        try
        {
            DataTable resultado = _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
            var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as GrupoEquipoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }

    private Result<CrearGrupoEquipoComando> ValidarEntrada(CrearGrupoEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Modelo))
            errors.Add(new("Modelo", "El modelo es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Marca))
            errors.Add(new("Marca", "La marca es requerida"));

        if (string.IsNullOrWhiteSpace(comando?.Descripcion))
            errors.Add(new("Descripcion", "La descripción es requerida"));

        if (string.IsNullOrWhiteSpace(comando?.NombreCategoria))
            errors.Add(new("NombreCategoria", "La categoría es requerida"));

        if (string.IsNullOrWhiteSpace(comando?.UrlImagen))
            errors.Add(new("UrlImagen", "La URL de la imagen es requerida"));

        return errors.Any()
            ? Result<CrearGrupoEquipoComando>.Invalid(errors.ToArray())
            : Result<CrearGrupoEquipoComando>.Success(comando!);
    }

    private Result<ActualizarGrupoEquipoComando> ValidarEntrada(ActualizarGrupoEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Modelo) && comando.Modelo.Length > 255)
            errors.Add(new("Modelo", "El modelo no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Marca) && comando.Marca.Length > 255)
            errors.Add(new("Marca", "La marca no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Descripcion) && comando.Descripcion.Length > 255)
            errors.Add(new("Descripcion", "La descripción no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.NombreCategoria) && comando.NombreCategoria.Length > 255)
            errors.Add(new("NombreCategoria", "El nombre de la categoría no puede tener más de 255 caracteres"));

        return errors.Any()
            ? Result<ActualizarGrupoEquipoComando>.Invalid(errors.ToArray())
            : Result<ActualizarGrupoEquipoComando>.Success(comando!);
    }

    private Result<EliminarGrupoEquipoComando> ValidarEntrada(EliminarGrupoEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarGrupoEquipoComando>.Invalid(errors.ToArray())
            : Result<EliminarGrupoEquipoComando>.Success(comando!);
    }

    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new GrupoEquipoDto
        {
            Id = Convert.ToInt32(fila["id_grupo_equipo"]),
            Nombre = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            Modelo = fila["modelo_grupo_equipo"] == DBNull.Value ? null : fila["modelo_grupo_equipo"].ToString(),
            Marca = fila["marca_grupo_equipo"] == DBNull.Value ? null : fila["marca_grupo_equipo"].ToString(),
            Descripcion = fila["descripcion_grupo_equipo"] == DBNull.Value ? null : fila["descripcion_grupo_equipo"].ToString(),
            NombreCategoria = fila["nombre_categoria"] == DBNull.Value ? null : fila["nombre_categoria"].ToString(),
            UrlDataSheet = fila["url_data_sheet_grupo_equipo"] == DBNull.Value ? null : fila["url_data_sheet_grupo_equipo"].ToString(),
            UrlImagen = fila["url_imagen_grupo_equipo"] == DBNull.Value ? null : fila["url_imagen_grupo_equipo"].ToString(),
            Cantidad = fila["cantidad_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["cantidad_grupo_equipo"]),
            CostoPromedio = fila["costo_promedio"] == DBNull.Value ? null : Convert.ToDecimal(fila["costo_promedio"])
        };
    }
}
