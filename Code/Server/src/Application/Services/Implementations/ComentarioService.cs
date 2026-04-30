using System.Data;
using Ardalis.Result;

public class ComentarioService : BaseServicios, IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ComentarioService(IComentarioRepository comentarioRepository, IUsuarioRepository usuarioRepository)
    {
        _comentarioRepository = comentarioRepository;
        _usuarioRepository = usuarioRepository;
    }

    public Result<ComentarioDto> Crear(CrearComentarioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<ComentarioDto>.Invalid(validResult.ValidationErrors.ToArray());

        var result = _comentarioRepository.Crear(comando);
        return result;
    }

    private Result<CrearComentarioComando> ValidarEntrada(CrearComentarioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.CarnetUsuario))
            errors.Add(new("CarnetUsuario", "El carnet del usuario es requerido"));

        if (comando?.IdGrupoEquipo <= 0)
            errors.Add(new("IdGrupoEquipo", "El ID del grupo de equipo es inválido"));

        if (string.IsNullOrWhiteSpace(comando?.Contenido))
            errors.Add(new("Contenido", "El contenido es requerido"));

        if (comando?.Contenido?.Length > 500)
            errors.Add(new("Contenido", "El contenido no puede exceder 500 caracteres"));

        return errors.Any()
            ? Result<CrearComentarioComando>.Invalid(errors.ToArray())
            : Result<CrearComentarioComando>.Success(comando!);
    }

    public List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta)
    {
        if (consulta == null) throw new ArgumentNullException();
        if (consulta.IdGrupoEquipo <= 0) throw new ArgumentException("El ID del grupo de equipo es inválido");

        var comentariosData = _comentarioRepository.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo);
        if (comentariosData.Rows.Count == 0) return new List<ComentarioDto>();

        List<string> carnets = new List<string>();
        for (int i = 0; i < comentariosData.Rows.Count; i++)
        {
            var carnet = comentariosData.Rows[i]["carnet_usuario"]?.ToString();
            if (!string.IsNullOrEmpty(carnet) && !carnets.Contains(carnet))
                carnets.Add(carnet);
        }

        var usuariosMap = new Dictionary<string, (string Nombre, string Apellido)>();
        if (carnets.Count > 0)
        {
            var usuariosData = _usuarioRepository.ObtenerPorCarnets(carnets);
            for (int i = 0; i < usuariosData.Rows.Count; i++)
            {
                var row = usuariosData.Rows[i];
                var carnet = row["carnet"]?.ToString();
                var nombre = row["nombre"]?.ToString() ?? string.Empty;
                var apellido = row["apellido_paterno"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(carnet) && !usuariosMap.ContainsKey(carnet))
                    usuariosMap[carnet] = (nombre, apellido);
            }
        }

        var lista = new List<ComentarioDto>(comentariosData.Rows.Count);
        for (int i = 0; i < comentariosData.Rows.Count; i++)
        {
            var baseDto = MapearFilaADto(comentariosData.Rows[i]);
            if (baseDto is ComentarioDto dto)
            {
                if (!string.IsNullOrEmpty(dto.CarnetUsuario) && usuariosMap.ContainsKey(dto.CarnetUsuario))
                {
                    var usuario = usuariosMap[dto.CarnetUsuario];
                    dto.NombreUsuario = usuario.Nombre;
                    dto.ApellidoPaternoUsuario = usuario.Apellido;
                }
                lista.Add(dto);
            }
        }
        return lista;
    }

    public Result<ComentarioDto> Eliminar(EliminarComentarioComando comando)
    {
        var validResult = ValidarEntradaEliminar(comando);
        if (!validResult.IsSuccess) return Result<ComentarioDto>.Invalid(validResult.ValidationErrors.ToArray());

        var result = _comentarioRepository.Eliminar(comando);
        return result;
    }

    private Result<EliminarComentarioComando> ValidarEntradaEliminar(EliminarComentarioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Id))
            errors.Add(new("Id", "El ID del comentario es inválido"));

        return errors.Any()
            ? Result<EliminarComentarioComando>.Invalid(errors.ToArray())
            : Result<EliminarComentarioComando>.Success(comando!);
    }

    public void AgregarLikeComentario(AgregarLikeComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ArgumentException("El ID del comentario es requerido");
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ArgumentException("El carnet del usuario es requerido");

        _comentarioRepository.AgregarLike(comando);
    }

    public void QuitarLikeComentario(QuitarLikeComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ArgumentException("El ID del comentario es requerido");
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ArgumentException("El carnet del usuario es requerido");

        _comentarioRepository.QuitarLike(comando);
    }

    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new ComentarioDto
        {
            Id = fila["id_comentario"].ToString() ?? string.Empty,
            CarnetUsuario = fila["carnet_usuario"] == DBNull.Value ? null : fila["carnet_usuario"].ToString(),
            IdGrupoEquipo = fila["id_grupo_equipo"] == DBNull.Value ? 0 : Convert.ToInt32(fila["id_grupo_equipo"]),
            Contenido = fila["contenido_comentario"] == DBNull.Value ? null : fila["contenido_comentario"].ToString(),
            Likes = fila["likes_comentario"] == DBNull.Value ? 0 : Convert.ToInt32(fila["likes_comentario"]),
            FechaCreacion = fila["fecha_creacion_comentario"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_creacion_comentario"])
        };
    }
}
