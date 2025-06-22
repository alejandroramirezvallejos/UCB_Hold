using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class GrupoEquipoService : IGrupoEquipoService
{
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;

    public GrupoEquipoService(IGrupoEquipoRepository grupoEquipoRepository) => _grupoEquipoRepository = grupoEquipoRepository;

    public void CrearGrupoEquipo(CrearGrupoEquipoComando comando)
    {
        ValidarEntradaCreacion(comando);
        _grupoEquipoRepository.Crear(comando);
    }

    public void ActualizarGrupoEquipo(ActualizarGrupoEquipoComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _grupoEquipoRepository.Actualizar(comando);
    }

    public void EliminarGrupoEquipo(EliminarGrupoEquipoComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _grupoEquipoRepository.Eliminar(comando.Id);
    }

    public GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        if (consulta == null || consulta.Id <= 0) throw new ArgumentException();
        var resultado = _grupoEquipoRepository.ObtenerPorId(consulta.Id);
        if (resultado?.Rows.Count > 0) return MapearFilaADto(resultado.Rows[0]);
        return null;
    }

    public List<GrupoEquipoDto>? ObtenerTodosGruposEquipos()
    {
        var resultado = _grupoEquipoRepository.ObtenerTodos();
        var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    public List<GrupoEquipoDto>? ObtenerGrupoEquipoPorNombreYCategoria(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta)
    {
        var resultado = _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
        var lista = new List<GrupoEquipoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    private void ValidarEntradaCreacion(CrearGrupoEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (string.IsNullOrWhiteSpace(comando.Modelo)) throw new ErrorModeloRequerido();
        if (string.IsNullOrWhiteSpace(comando.Marca)) throw new ErrorMarcaRequerida();
        if (string.IsNullOrWhiteSpace(comando.Descripcion)) throw new ErrorDescripcionRequerida();
        if (string.IsNullOrWhiteSpace(comando.NombreCategoria)) throw new ErrorCategoriaRequerida();
        if (string.IsNullOrWhiteSpace(comando.UrlImagen)) throw new ErrorUrlImagenRequerida();
    }
    private void ValidarEntradaActualizacion(ActualizarGrupoEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Marca) && comando.Marca.Length > 255) throw new ErrorLongitudInvalida("marca grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.Descripcion) && comando.Descripcion.Length > 255) throw new ErrorLongitudInvalida("descripcion grupo equipo", 255);
        if (!string.IsNullOrWhiteSpace(comando.NombreCategoria) && comando.NombreCategoria.Length > 255) throw new ErrorLongitudInvalida("nombre categoria grupo equipo", 255);
    }
    private void ValidarEntradaEliminacion(EliminarGrupoEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static GrupoEquipoDto MapearFilaADto(DataRow fila) => new GrupoEquipoDto
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