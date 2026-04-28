using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class GrupoEquipoService : BaseServicios,
    ICrearServicio<CrearGrupoEquipoComando>,
    IActualizarServicio<ActualizarGrupoEquipoComando>,
    IEliminarServicio<EliminarGrupoEquipoComando>,
    IObtenerTodosServicio<GrupoEquipoDto>
{
    private readonly GrupoEquipoRepository _grupoEquipoRepository;
    public GrupoEquipoService(GrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }
    public virtual void Crear(CrearGrupoEquipoComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: nombre de categoría → id_categoria
        var idCategoria = _grupoEquipoRepository.ObtenerCategoriaIdPorNombre(comando.NombreCategoria!);
        if (idCategoria == null)
            throw new ErrorCategoriaNoEncontrada();

        // Verificar duplicados por combinación nombre+modelo+marca
        if (_grupoEquipoRepository.ExisteDuplicadoPorNombreModeloMarca(comando.Nombre!, comando.Modelo!, comando.Marca!))
            throw new ErrorRegistroYaExiste();

        _grupoEquipoRepository.Crear(idCategoria.Value, comando);
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
    
    public virtual List<GrupoEquipoDto>? ObtenerTodos()
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
    public virtual void Actualizar(ActualizarGrupoEquipoComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el grupo exista y esté activo
        if (!_grupoEquipoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        int? idCategoria = null;

        // Resolver FK de categoría si se está cambiando
        if (!string.IsNullOrWhiteSpace(comando.NombreCategoria))
        {
            idCategoria = _grupoEquipoRepository.ObtenerCategoriaIdPorNombre(comando.NombreCategoria);
            if (idCategoria == null)
                throw new ErrorCategoriaNoEncontrada();
        }

        // Verificar duplicados si se cambiaron nombre, modelo o marca
        if (!string.IsNullOrWhiteSpace(comando.Nombre) || !string.IsNullOrWhiteSpace(comando.Modelo) || !string.IsNullOrWhiteSpace(comando.Marca))
        {
            // Obtener datos actuales para rellenar los que no se están cambiando
            var actual = _grupoEquipoRepository.ObtenerPorId(comando.Id);
            if (actual != null && actual.Rows.Count > 0)
            {
                var nombreFinal = !string.IsNullOrWhiteSpace(comando.Nombre) ? comando.Nombre : actual.Rows[0]["nombre_grupo_equipo"]?.ToString();
                var modeloFinal = !string.IsNullOrWhiteSpace(comando.Modelo) ? comando.Modelo : actual.Rows[0]["modelo_grupo_equipo"]?.ToString();
                var marcaFinal = !string.IsNullOrWhiteSpace(comando.Marca) ? comando.Marca : actual.Rows[0]["marca_grupo_equipo"]?.ToString();

                if (nombreFinal != null && modeloFinal != null && marcaFinal != null)
                {
                    if (_grupoEquipoRepository.ExisteDuplicadoPorNombreModeloMarcaExcluyendoId(nombreFinal, modeloFinal, marcaFinal, comando.Id))
                        throw new ErrorRegistroYaExiste();
                }
            }
        }

        _grupoEquipoRepository.Actualizar(idCategoria, comando);
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
    public virtual void Eliminar(EliminarGrupoEquipoComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el grupo exista y esté activo
        if (!_grupoEquipoRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _grupoEquipoRepository.Eliminar(comando);
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