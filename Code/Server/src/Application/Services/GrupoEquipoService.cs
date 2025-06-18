using System.Data;

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
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_grupo_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Categoría no encontrada
                if (mensaje.Contains("no se encontró la categoría con nombre"))
                {
                    throw new ErrorReferenciaInvalida("categoría");
                }
                
                // Error: Grupo de equipos duplicado (combinación nombre+modelo+marca)
                if (mensaje.Contains("ya existe un grupo de equipos con nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error: Violación de unicidad
                if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al intentar insertar grupo de equipos"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar grupo de equipos"))
                {
                    throw new Exception($"Error inesperado al insertar grupo de equipos: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear grupo de equipos: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear grupo de equipos: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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
            throw new ErrorMarcaRequerida();

        if (string.IsNullOrWhiteSpace(comando.Descripcion))
            throw new ErrorDescripcionRequerida();

        if (string.IsNullOrWhiteSpace(comando.NombreCategoria))
            throw new ErrorCategoriaRequerida();

        if (string.IsNullOrWhiteSpace(comando.UrlImagen))
            throw new ErrorUrlImagenRequerida();
    }    private void ValidarEntradaActualizacion(ActualizarGrupoEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();

        if (string.IsNullOrWhiteSpace(comando.Marca))
            throw new ErrorMarcaRequerida();

        if (string.IsNullOrWhiteSpace(comando.Descripcion))
            throw new ErrorDescripcionRequerida();

        if (string.IsNullOrWhiteSpace(comando.NombreCategoria))
            throw new ErrorCategoriaRequerida();

        if (string.IsNullOrWhiteSpace(comando.UrlImagen))
            throw new ErrorUrlImagenRequerida();
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
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_grupo_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Grupo de equipos no encontrado
                if (mensaje.Contains("no se encontró un grupo de equipos activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error: Categoría no encontrada
                if (mensaje.Contains("no se encontró la categoría activa con nombre"))
                {
                    throw new ErrorReferenciaInvalida("categoría");
                }
                
                // Error: Combinación duplicada (nombre+modelo+marca)
                if (mensaje.Contains("ya existe otro grupo de equipos activo con la combinación"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error: Violación de unicidad específica
                if (errorDb.SqlState == "23505" || mensaje.Contains("la combinación nombre") || mensaje.Contains("ya está en uso"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error inesperado al actualizar el grupo de equipos"))
                {
                    throw new Exception($"Error inesperado al actualizar grupo de equipos: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al actualizar grupo de equipos: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar grupo de equipos: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_grupo_equipo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Grupo de equipos no encontrado
                if (mensaje.Contains("no se encontró un grupo de equipos activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el grupo de equipos"))
                {
                    throw new Exception($"Error inesperado al eliminar grupo de equipos: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar grupo de equipos: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar grupo de equipos: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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