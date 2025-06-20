using System.Data;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Server.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ComentarioService(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public void CrearComentario(CrearComentarioComando comando)
        {
            try
            {
                ValidarEntradaCreacion(comando);
                _comentarioRepository.Crear(comando);
            }
            catch (ErrorCarnetInvalido)
            {
                throw;
            }
            catch (ErrorIdInvalido)
            {
                throw;
            }
            catch (ErrorCampoRequerido)
            {
                throw;
            }
            catch (ErrorLongitudInvalida)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ErrorDataBase errorDb)
                {
                    var mensaje = errorDb.Message?.ToLower() ?? "";
                    
                    if (mensaje.Contains("no se encontró el usuario con carnet"))
                    {
                        throw new ErrorReferenciaInvalida("usuario");
                    }
                    
                    if (mensaje.Contains("no se encontró el grupo de equipos con id"))
                    {
                        throw new ErrorReferenciaInvalida("grupo de equipos");
                    }
                    
                    if (mensaje.Contains("error al insertar comentario"))
                    {
                        throw new Exception($"Error inesperado al insertar comentario: {errorDb.Message}", errorDb);
                    }
                    
                    throw new Exception($"Error inesperado de base de datos al crear comentario: {errorDb.Message}", errorDb);
                }
                
                if (ex is ErrorRepository errorRepo)
                {
                    throw new Exception($"Error del repositorio al crear comentario: {errorRepo.Message}", errorRepo);
                }
                
                throw;
            }
        }

        public List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta)
        {
            try
            {
                if (consulta == null)
                    throw new ArgumentNullException(nameof(consulta), "La consulta es requerida");

                if (consulta.IdGrupoEquipo <= 0)
                    throw new ArgumentException("El ID del grupo de equipos debe ser mayor a 0", nameof(consulta.IdGrupoEquipo));

                DataTable resultado = _comentarioRepository.ObtenerPorGrupoEquipo(consulta.IdGrupoEquipo);
                var lista = new List<ComentarioDto>(resultado.Rows.Count);
                foreach (DataRow fila in resultado.Rows)
                {
                    lista.Add(MapearFilaADto(fila));
                }
                return lista;
            }
            catch
            {
                throw;
            }
        }
        
        public void EliminarComentario(EliminarComentarioComando comando)
        {
            try
            {
                ValidarEntradaEliminacion(comando);
                _comentarioRepository.Eliminar(comando);
            }
            catch (ErrorIdInvalido)
            {
                throw;
            }
            catch (ErrorCarnetInvalido)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ErrorDataBase errorDb)
                {
                    var mensaje = errorDb.Message?.ToLower() ?? "";

                    if (mensaje.Contains("id de comentario inválido"))
                    {
                        throw new ErrorIdInvalido(errorDb.Message);
                    }
                    
                    if (mensaje.Contains("no se encontró el comentario"))
                    {
                        throw new ErrorRegistroNoEncontrado();
                    }
                    
                    if (mensaje.Contains("el usuario con carnet") && mensaje.Contains("no es el propietario del comentario"))
                    {
                        throw new ErrorUsuarioNoAutorizado();
                    }
                    
                    if (mensaje.Contains("error al eliminar lógicamente el comentario"))
                    {
                        throw new Exception($"Error inesperado al eliminar comentario: {errorDb.Message}", errorDb);
                    }
                    
                    throw new Exception($"Error inesperado de base de datos al eliminar comentario: {errorDb.Message}", errorDb);
                }
                
                if (ex is ErrorRepository errorRepo)
                {
                    throw new Exception($"Error del repositorio al eliminar comentario: {errorRepo.Message}", errorRepo);
                }
                
                throw;
            }
        }

        public void AgregarLikeComentario(AgregarLikeComentarioComando comando)
        {
            try
            {
                ValidarEntradaLike(comando);
                _comentarioRepository.AgregarLike(comando);
            }
            catch (ErrorIdInvalido)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ErrorDataBase errorDb)
                {
                    var mensaje = errorDb.Message?.ToLower() ?? "";
                    
                    if (mensaje.Contains("no se encontró el comentario"))
                    {
                        throw new ErrorRegistroNoEncontrado();
                    }
                    
                    if (mensaje.Contains("error al agregar like al comentario"))
                    {
                        throw new Exception($"Error inesperado al agregar like al comentario: {errorDb.Message}", errorDb);
                    }
                    
                    throw new Exception($"Error inesperado de base de datos al agregar like al comentario: {errorDb.Message}", errorDb);
                }
                
                if (ex is ErrorRepository errorRepo)
                {
                    throw new Exception($"Error del repositorio al agregar like al comentario: {errorRepo.Message}", errorRepo);
                }
                
                throw;
            }
        }

        private ComentarioDto MapearFilaADto(DataRow fila)
        {
            return new ComentarioDto
            {
                Id = fila["id_comentario"].ToString(),
                CarnetUsuario = fila["carnet_usuario"] == DBNull.Value ? null : fila["carnet_usuario"].ToString(),
                NombreUsuario = fila["nombre_usuario"] == DBNull.Value ? null : fila["nombre_usuario"].ToString(),
                ApellidoPaternoUsuario = fila["apellido_paterno_usuario"] == DBNull.Value ? null : fila["apellido_paterno_usuario"].ToString(),
                IdGrupoEquipo = fila["id_grupo_equipo"] == DBNull.Value ? 0 : Convert.ToInt32(fila["id_grupo_equipo"]),
                Contenido = fila["contenido_comentario"] == DBNull.Value ? null : fila["contenido_comentario"].ToString(),
                Likes = fila["likes_comentario"] == DBNull.Value ? 0 : Convert.ToInt32(fila["likes_comentario"]),
                FechaCreacion = fila["fecha_creacion_comentario"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_creacion_comentario"])
            };
        }

        private void ValidarEntradaCreacion(CrearComentarioComando comando)
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando));

            if (string.IsNullOrWhiteSpace(comando.CarnetUsuario))
                throw new ErrorCarnetInvalido();

            if (comando.IdGrupoEquipo <= 0)
                throw new ErrorIdInvalido("El IdGrupoEquipo es requerido y debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(comando.Contenido))
                throw new ErrorCampoRequerido("contenido");

            if (comando.Contenido.Length > 500)
                throw new ErrorLongitudInvalida("contenido", 500);
        }

        private void ValidarEntradaEliminacion(EliminarComentarioComando comando)
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando));

            if (string.IsNullOrWhiteSpace(comando.Id))
                throw new ErrorIdInvalido();
        }

        private void ValidarEntradaLike(AgregarLikeComentarioComando comando)
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando));

            if (string.IsNullOrWhiteSpace(comando.Id))
                throw new ErrorIdInvalido();
        }
    }
}