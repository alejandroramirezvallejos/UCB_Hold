using System.Data;
using System.Linq;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

public class ComentarioService : BaseServicios, IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    
    public ComentarioService(IComentarioRepository comentarioRepository, IUsuarioRepository usuarioRepository)
    {
        _comentarioRepository = comentarioRepository;
        _usuarioRepository = usuarioRepository;
    }

    public void CrearComentario(CrearComentarioComando comando)
    {
        ValidarEntradaCreacion(comando);
        _comentarioRepository.Crear(comando);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearComentarioComando
        if (comando is CrearComentarioComando comentarioComando)
        {
            if (string.IsNullOrWhiteSpace(comentarioComando.CarnetUsuario)) throw new ErrorCarnetInvalido();
            if (comentarioComando.IdGrupoEquipo <= 0) throw new ErrorIdInvalido("grupo equipo");
            if (string.IsNullOrWhiteSpace(comentarioComando.Contenido)) throw new ErrorCampoRequerido("contenido");
            if (comentarioComando.Contenido.Length > 500) throw new ErrorLongitudInvalida("contenido", 500);
        }
    }

    public List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta)
    {
        if (consulta == null) throw new ArgumentNullException();
        if (consulta.IdGrupoEquipo <= 0) throw new ErrorIdInvalido("comentarios");
        
        var comentariosData = _comentarioRepository.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo);
        if (comentariosData.Rows.Count == 0) return new List<ComentarioDto>();
        
        var carnets = comentariosData.AsEnumerable()
            .Select(r => r.Field<string>("carnet_usuario"))
            .Where(c => !string.IsNullOrEmpty(c))
            .OfType<string>()
            .Distinct()
            .ToList();
        
        var usuariosMap = new Dictionary<string, (string Nombre, string Apellido)>();
        if (carnets.Any())
        {
            var usuariosData = _usuarioRepository.ObtenerPorCarnets(carnets);
            usuariosMap = usuariosData.AsEnumerable().ToDictionary(
                row => row.Field<string>("carnet")!,
                row => (row.Field<string>("nombre")!, row.Field<string>("apellido_paterno")!)
            );
        }
        
        var lista = new List<ComentarioDto>(comentariosData.Rows.Count);
        foreach (DataRow fila in comentariosData.Rows)
        {
            var baseDto = MapearFilaADto(fila);
            if (baseDto is ComentarioDto dto)
            {
                if (dto.CarnetUsuario != null && usuariosMap.ContainsKey(dto.CarnetUsuario))
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

    public void EliminarComentario(EliminarComentarioComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _comentarioRepository.Eliminar(comando);
    }
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)

        // Validaciones específicas para EliminarComentarioComando
        if (comando is EliminarComentarioComando comentarioComando)
        {
            if (string.IsNullOrWhiteSpace(comentarioComando.Id)) throw new ErrorIdInvalido("comentario");
        }
    }

    public void AgregarLikeComentario(AgregarLikeComentarioComando comando)
    {
        ValidarEntradaLike(comando);
        _comentarioRepository.AgregarLike(comando);
    }
    
    protected virtual void ValidarEntradaLike(AgregarLikeComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ErrorIdInvalido("comentario");
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ErrorCarnetInvalido();
    }
    
    public void QuitarLikeComentario(QuitarLikeComentarioComando comando)
    {
        ValidarEntradaQuitarLike(comando);
        _comentarioRepository.QuitarLike(comando);
    }
    
    protected virtual void ValidarEntradaQuitarLike(QuitarLikeComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ErrorIdInvalido("comentario");
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ErrorCarnetInvalido();
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
