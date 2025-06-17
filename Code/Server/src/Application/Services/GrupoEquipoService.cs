using System.Data;
using Shared.Common;

public class GrupoEquipoService : IGrupoEquipoService
{
    private readonly GrupoEquipoRepository _grupoEquipoRepository;

    public GrupoEquipoService(GrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }    public void CrearGrupoEquipo(CrearGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _grupoEquipoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorModeloRequerido)
        {
            throw;
        }
        catch (ErrorCampoRequerido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre,
                ["modelo"] = comando.Modelo,
                ["marca"] = comando.Marca,
                ["categoria"] = comando.NombreCategoria
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "grupo de equipo", parametros);
        }
    }private void ValidarEntradaCreacion(CrearGrupoEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();

        if (string.IsNullOrWhiteSpace(comando.Marca))
            throw new ErrorCampoRequerido("marca");

        if (string.IsNullOrWhiteSpace(comando.Descripcion))
            throw new ErrorCampoRequerido("descripcion");

        if (string.IsNullOrWhiteSpace(comando.NombreCategoria))
            throw new ErrorCampoRequerido("categoria");

        if (string.IsNullOrWhiteSpace(comando.UrlImagen))
            throw new ErrorCampoRequerido("url de imagen");
    }    private void ValidarEntradaActualizacion(ActualizarGrupoEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();

        if (string.IsNullOrWhiteSpace(comando.Marca))
            throw new ErrorCampoRequerido("marca");

        if (string.IsNullOrWhiteSpace(comando.Descripcion))
            throw new ErrorCampoRequerido("descripcion");

        if (string.IsNullOrWhiteSpace(comando.NombreCategoria))
            throw new ErrorCampoRequerido("categoria");

        if (string.IsNullOrWhiteSpace(comando.UrlImagen))
            throw new ErrorCampoRequerido("url de imagen");
    }

    private void ValidarEntradaEliminacion(EliminarGrupoEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }public GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        try
        {
            if (consulta == null)
                throw new ArgumentNullException(nameof(consulta), "La consulta es requerida");

            if (consulta.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(consulta.Id));

            DataTable? resultado = _grupoEquipoRepository.ObtenerPorId(consulta.Id);
            if (resultado?.Rows.Count > 0)
            {
                return MapearFilaADto(resultado.Rows[0]);
            }
            return null;
        }
        catch
        {
            throw;
        }
    }
    public List<GrupoEquipoDto>? ObtenerTodosGruposEquipos()
    {
        try
        {
            DataTable resultado = _grupoEquipoRepository.ObtenerTodos();
            var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
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
    public List<GrupoEquipoDto>? ObtenerGrupoEquipoPorNombreYCategoria(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta)
    {
        try
        {
            DataTable resultado = _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
            var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
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
    }    public void ActualizarGrupoEquipo(ActualizarGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _grupoEquipoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorModeloRequerido)
        {
            throw;
        }
        catch (ErrorCampoRequerido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.Nombre,
                ["modelo"] = comando.Modelo,
                ["marca"] = comando.Marca,
                ["categoria"] = comando.NombreCategoria
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "grupo de equipo", parametros);
        }
    }
    public void EliminarGrupoEquipo(EliminarGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _grupoEquipoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "grupo de equipo", parametros);
        }
    }
    private GrupoEquipoDto MapearFilaADto(DataRow fila)
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
            Cantidad = fila["cantidad_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["cantidad_grupo_equipo"])
        };
    }
}