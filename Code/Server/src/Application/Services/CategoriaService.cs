using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class CategoriaService : ICategoriaService
{
    private readonly CategoriaRepository _categoriaRepository;
    public CategoriaService(CategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }    
    public void CrearCategoria(CrearCategoriaComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _categoriaRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_categoria según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Nombre vacío (aunque ya se valida en la entrada)
                if (mensaje.Contains("el nombre de la categoría no puede estar vacío"))
                {
                    throw new ErrorNombreRequerido();
                }
                
                // Error: Nombre duplicado
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe una categoría con el nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar categoría"))
                {
                    throw new Exception($"Error inesperado al insertar categoría: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear categoría: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear categoría: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }    private void ValidarEntradaCreacion(CrearCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 50)
            throw new ErrorLongitudInvalida("nombre de la categoría", 50);
    }
    public List<CategoriaDto>? ObtenerTodasCategorias()
    {
        try
        {
            DataTable resultado = _categoriaRepository.ObtenerTodos();
            var lista = new List<CategoriaDto>(resultado.Rows.Count);
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
    }    public void ActualizarCategoria(ActualizarCategoriaComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _categoriaRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_categoria según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Categoría no encontrada
                if (mensaje.Contains("no se encontró una categoría activa con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error: Nombre vacío (aunque ya se valida en la entrada)
                if (mensaje.Contains("el nuevo nombre de la categoría no puede estar vacío"))
                {
                    throw new ErrorNombreRequerido();
                }
                
                // Error: Nombre duplicado
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe otra categoría con el nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error inesperado al actualizar la categoría"))
                {
                    throw new Exception($"Error inesperado al actualizar categoría: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al actualizar categoría: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar categoría: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    public void EliminarCategoria(EliminarCategoriaComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _categoriaRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_categoria según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Categoría no encontrada
                if (mensaje.Contains("no se encontró una categoría activa con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente la categoría"))
                {
                    throw new Exception($"Error inesperado al eliminar categoría: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar categoría: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar categoría: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }

    private void ValidarEntradaActualizacion(ActualizarCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 50)
            throw new ErrorLongitudInvalida("nombre de la categoría", 50);
    }    private void ValidarEntradaEliminacion(EliminarCategoriaComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
    private static CategoriaDto MapearFilaADto(DataRow fila)
    {
        return new CategoriaDto
        {
            Id = Convert.ToInt32(fila["id_categoria"]),
            Nombre = fila["categoria"] == DBNull.Value ? null : fila["categoria"].ToString(),
        };
    }
}