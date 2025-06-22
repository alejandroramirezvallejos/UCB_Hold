using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    public CategoriaService(ICategoriaRepository categoriaRepository) => _categoriaRepository = categoriaRepository;

    public void CrearCategoria(CrearCategoriaComando comando)
    {
        ValidarEntradaCreacion(comando);
        _categoriaRepository.Crear(comando);
    }

    public List<CategoriaDto>? ObtenerTodasCategorias()
    {
        DataTable resultado = _categoriaRepository.ObtenerTodos();
        var lista = new List<CategoriaDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    public void ActualizarCategoria(ActualizarCategoriaComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _categoriaRepository.Actualizar(comando);
    }

    public void EliminarCategoria(EliminarCategoriaComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _categoriaRepository.Eliminar(comando.Id);
    }

    private void ValidarEntradaCreacion(CrearCategoriaComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 50) throw new ErrorLongitudInvalida("nombre de la categoría", 50);
    }
    private void ValidarEntradaActualizacion(ActualizarCategoriaComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 50) throw new ErrorLongitudInvalida("nombre de la categoría", 50);
    }
    private void ValidarEntradaEliminacion(EliminarCategoriaComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static CategoriaDto MapearFilaADto(DataRow fila) => new CategoriaDto
    {
        Id = Convert.ToInt32(fila["id_categoria"]),
        Nombre = fila["categoria"] == DBNull.Value ? null : fila["categoria"].ToString(),
    };
}