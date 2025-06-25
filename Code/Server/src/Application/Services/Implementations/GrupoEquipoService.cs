using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class GrupoEquipoService : BaseServicios, IGrupoEquipoService
{
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;
    public GrupoEquipoService(IGrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }
    public virtual void CrearGrupoEquipo(CrearGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _grupoEquipoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCampoRequerido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearGrupoEquipoComando
        if (comando is CrearGrupoEquipoComando grupoComando)
        {
            if (string.IsNullOrWhiteSpace(grupoComando.Nombre)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(grupoComando.Modelo)) throw new ErrorModeloRequerido();
            if (string.IsNullOrWhiteSpace(grupoComando.Marca)) throw new ErrorMarcaRequerida();
            if (string.IsNullOrWhiteSpace(grupoComando.Descripcion)) throw new ErrorDescripcionRequerida();
            if (string.IsNullOrWhiteSpace(grupoComando.NombreCategoria)) throw new ErrorCategoriaRequerida();
            if (string.IsNullOrWhiteSpace(grupoComando.UrlImagen)) throw new ErrorUrlImagenRequerida();
        }
    }
    
    protected override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró la categoría con nombre")) throw new ErrorCategoriaNoEncontrada();
            if (mensaje.Contains("ya existe un grupo de equipos con nombre")) throw new ErrorRegistroYaExiste();
            if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al intentar insertar grupo de equipos")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar grupo de equipos")) throw new Exception($"Error inesperado al insertar grupo de equipos: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear grupo de equipos: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear grupo de equipos: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
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
    public virtual List<GrupoEquipoDto>? ObtenerTodosGruposEquipos()
    {
        try
        {
            DataTable resultado = _grupoEquipoRepository.ObtenerTodos();
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
    public virtual void ActualizarGrupoEquipo(ActualizarGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _grupoEquipoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCampoRequerido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorActualizacion(comando, ex);
        }
    }
    private void ValidarEntradaActualizacion(ActualizarGrupoEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("grupo de equipos");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Marca) && comando.Marca.Length > 255) throw new ErrorLongitudInvalida("marca grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Descripcion) && comando.Descripcion.Length > 255) throw new ErrorLongitudInvalida("descripcion grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.NombreCategoria) && comando.NombreCategoria.Length > 255) throw new ErrorLongitudInvalida("nombre categoria grupo equipo", 255);
    }
    public virtual void EliminarGrupoEquipo(EliminarGrupoEquipoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _grupoEquipoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarGrupoEquipoComando
        if (comando is EliminarGrupoEquipoComando grupoComando)
        {
            if (grupoComando.Id <= 0) throw new ErrorIdInvalido("grupo de equipos");
        }
    }
    
    protected override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un grupo de equipos activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el grupo de equipos")) throw new Exception($"Error inesperado al eliminar grupo de equipos: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar grupo de equipos: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar grupo de equipos: {errorRepo.Message}", errorRepo);        throw ex ?? new Exception("Error desconocido en eliminación");
    }
    
    private void InterpretarErrorActualizacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un grupo de equipos activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("no se encontró la categoría activa con nombre")) throw new ErrorCategoriaNoEncontrada();
            if (mensaje.Contains("ya existe otro grupo de equipos activo con la combinación")) throw new ErrorRegistroYaExiste();
            if (errorDb.SqlState == "23505" || mensaje.Contains("la combinación nombre") || mensaje.Contains("ya está en uso")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error inesperado al actualizar el grupo de equipos")) throw new Exception($"Error inesperado al actualizar grupo de equipos: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al actualizar grupo de equipos: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar grupo de equipos: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en actualización");
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
            Cantidad = fila["cantidad_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["cantidad_grupo_equipo"])
        };
    }
}