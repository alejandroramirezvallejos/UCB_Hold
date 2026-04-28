using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class CategoriaService : BaseServicios,
    ICrearServicio<CrearCategoriaComando>,
    IActualizarServicio<ActualizarCategoriaComando>,
    IEliminarServicio<EliminarCategoriaComando>,
    IObtenerTodosServicio<CategoriaDto>
{
    private readonly CategoriaRepository _categoriaRepository;

    public CategoriaService(CategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public virtual void Crear(CrearCategoriaComando comando)
    {
        ValidarEntradaCreacion(comando);

        var nombreTrimmed = comando.Nombre!.Trim();

        if (string.IsNullOrWhiteSpace(nombreTrimmed))
            throw new ErrorNombreRequerido();

        // Intentar reactivar si existe una categoría eliminada lógicamente
        if (_categoriaRepository.ReactivarEliminadaPorNombre(nombreTrimmed))
            return;

        // Verificar si ya existe una categoría activa con ese nombre
        if (_categoriaRepository.ExisteActivaPorNombre(nombreTrimmed))
            throw new ErrorRegistroYaExiste();

        // Insertar nueva categoría (crear nuevo record con nombre trimmed)
        var comandoFinal = new CrearCategoriaComando(nombreTrimmed);
        _categoriaRepository.Crear(comandoFinal);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearCategoriaComando
        if (comando is CrearCategoriaComando categoriaComando)
        {
            if (string.IsNullOrWhiteSpace(categoriaComando.Nombre)) throw new ErrorNombreRequerido();
            if (categoriaComando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre de la categoría", 255);
        }
    }
    public virtual List<CategoriaDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _categoriaRepository.ObtenerTodos();
            var lista = new List<CategoriaDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var baseDto = MapearFilaADto(fila);
                if (baseDto is CategoriaDto categoria)
                    lista.Add(categoria);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void Actualizar(ActualizarCategoriaComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que la categoría exista y esté activa
        if (!_categoriaRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        var nombreNuevo = comando.Nombre?.Trim();

        if (nombreNuevo != null)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevo))
                throw new ErrorNombreRequerido();

            // Verificar si ya existe otra categoría activa con ese nombre
            if (_categoriaRepository.ExisteActivaPorNombreExcluyendoId(nombreNuevo, comando.Id))
                throw new ErrorRegistroYaExiste();

            // Si existe una categoría eliminada con el mismo nombre, reactivarla y eliminar la actual
            if (_categoriaRepository.ReactivarEliminadaPorNombre(nombreNuevo))
            {
                _categoriaRepository.EliminarLogicamentePorId(comando.Id);
                return;
            }
        }

        // Actualización normal
        var comandoFinal = new ActualizarCategoriaComando(comando.Id, nombreNuevo);
        _categoriaRepository.Actualizar(comandoFinal);
    }
    
    private void ValidarEntradaActualizacion(ActualizarCategoriaComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("categoría");
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre de la categoría", 255);
    }
    public virtual void Eliminar(EliminarCategoriaComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que la categoría exista y esté activa
        if (!_categoriaRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _categoriaRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarCategoriaComando
        if (comando is EliminarCategoriaComando categoriaComando)
        {
            if (categoriaComando.Id <= 0) throw new ErrorIdInvalido("categoría");
        }
    }
    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new CategoriaDto
        {
            Id = Convert.ToInt32(fila["id_categoria"]),
            Nombre = fila["categoria"] == DBNull.Value ? null : fila["categoria"].ToString(),
        };
    }
}