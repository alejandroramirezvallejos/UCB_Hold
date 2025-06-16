using System.Data;

public class GrupoEquipoService : IGrupoEquipoService
{
    private readonly GrupoEquipoRepository _grupoEquipoRepository;

    public GrupoEquipoService(GrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }
    public void CrearGrupoEquipo(CrearGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        try
        {
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
    }
    public void ActualizarGrupoEquipo(ActualizarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarGrupoEquipo(EliminarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Eliminar(comando.Id);
        }
        catch
        {
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