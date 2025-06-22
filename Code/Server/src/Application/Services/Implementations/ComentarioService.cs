using System.Data;
using System.Linq;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IExecuteQuery _executeQuery;
    public ComentarioService(IComentarioRepository comentarioRepository, IExecuteQuery executeQuery)
    {
        _comentarioRepository = comentarioRepository;
        _executeQuery = executeQuery;
    }

    public void CrearComentario(CrearComentarioComando comando)
    {
        ValidarEntradaCreacion(comando);
        _comentarioRepository.Crear(comando);
    }

    public List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta)
    {
        if (consulta == null) throw new ArgumentNullException();
        if (consulta.IdGrupoEquipo <= 0) throw new ErrorIdInvalido();
        var comentariosData = _comentarioRepository.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo);
        if (comentariosData.Rows.Count == 0) return new List<ComentarioDto>();
        var carnets = comentariosData.AsEnumerable().Select(r => r.Field<string>("carnet_usuario")).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
        var usuariosMap = new Dictionary<string, (string Nombre, string Apellido)>();
        if (carnets.Any())
        {
            var sql = "SELECT carnet, nombre, apellido_paterno FROM public.usuarios WHERE carnet = ANY(@carnets)";
            var parametros = new Dictionary<string, object?> { { "carnets", carnets } };
            var usuariosData = _executeQuery.EjecutarFuncion(sql, parametros);
            usuariosMap = usuariosData.AsEnumerable().ToDictionary(
                row => row.Field<string>("carnet"),
                row => (row.Field<string>("nombre"), row.Field<string>("apellido_paterno"))
            );
        }
        var lista = new List<ComentarioDto>(comentariosData.Rows.Count);
        foreach (DataRow fila in comentariosData.Rows)
        {
            var dto = MapearFilaADto(fila);
            if (dto.CarnetUsuario != null && usuariosMap.ContainsKey(dto.CarnetUsuario))
            {
                var usuario = usuariosMap[dto.CarnetUsuario];
                dto.NombreUsuario = usuario.Nombre;
                dto.ApellidoPaternoUsuario = usuario.Apellido;
            }
            lista.Add(dto);
        }
        return lista;
    }

    public void EliminarComentario(EliminarComentarioComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _comentarioRepository.Eliminar(comando);
    }

    public void AgregarLikeComentario(AgregarLikeComentarioComando comando)
    {
        ValidarEntradaLike(comando);
        _comentarioRepository.AgregarLike(comando);
    }

    private ComentarioDto MapearFilaADto(DataRow fila) => new ComentarioDto
    {
        Id = fila["id_comentario"].ToString(),
        CarnetUsuario = fila["carnet_usuario"] == DBNull.Value ? null : fila["carnet_usuario"].ToString(),
        IdGrupoEquipo = fila["id_grupo_equipo"] == DBNull.Value ? 0 : Convert.ToInt32(fila["id_grupo_equipo"]),
        Contenido = fila["contenido_comentario"] == DBNull.Value ? null : fila["contenido_comentario"].ToString(),
        Likes = fila["likes_comentario"] == DBNull.Value ? 0 : Convert.ToInt32(fila["likes_comentario"]),
        FechaCreacion = fila["fecha_creacion_comentario"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_creacion_comentario"])
    };

    private void ValidarEntradaCreacion(CrearComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ErrorCarnetInvalido();
        if (comando.IdGrupoEquipo <= 0) throw new ErrorIdInvalido();
        if (string.IsNullOrWhiteSpace(comando.Contenido)) throw new ErrorCampoRequerido("contenido");
        if (comando.Contenido.Length > 500) throw new ErrorLongitudInvalida("contenido", 500);
    }
    private void ValidarEntradaEliminacion(EliminarComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ErrorIdInvalido();
    }
    private void ValidarEntradaLike(AgregarLikeComentarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Id)) throw new ErrorIdInvalido();
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario)) throw new ErrorCarnetInvalido();
    }
}
