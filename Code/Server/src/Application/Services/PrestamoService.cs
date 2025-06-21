using System.Data;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;
using MongoDB.Driver;


public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly MongoDbContexto _mongoDbContexto;

    public PrestamoService(IPrestamoRepository prestamoRepository, MongoDbContexto mongoDbContexto)
    {
        _prestamoRepository = prestamoRepository;
        _mongoDbContexto = mongoDbContexto;
    }    public virtual void CrearPrestamo(CrearPrestamoComando comando)
    {
        try
        {
            // 1. Validaciones de entrada
            ValidarEntradaCreacion(comando);

            // Validar que el archivo sea HTML
            if (comando.Contrato != null)
            {
                if (!comando.Contrato.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("El contrato debe ser un archivo HTML (.html)");
            }
            // 2. Intentar crear en repository
            var prestamoId = _prestamoRepository.Crear(comando);

            // 3. Guardar contrato en MongoDB y actualizar prestamo
            if (comando.Contrato != null)
            {
                ObjectId fileId;
                using (var stream = comando.Contrato.OpenReadStream())
                {
                    fileId = _mongoDbContexto.GestionArchivos.UploadFromStreamAsync(comando.Contrato.FileName, stream).GetAwaiter().GetResult();
                }

                var contrato = new Contrato
                {
                    PrestamoId = prestamoId,
                    FileId = fileId.ToString()
                };
                _mongoDbContexto.Contratos.InsertOneAsync(contrato).GetAwaiter().GetResult();

                _prestamoRepository.ActualizarIdContrato(prestamoId, contrato.Id);
            }
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorIdInvalido)
        {
            throw; // Re-lanzar excepciones de validación  
        }
        catch (ErrorGrupoEquipoIdInvalido)
        {
            throw;
        }
        catch (ErrorFechaPrestamoYFechaDevolucionInvalidas)
        {
            throw;
        }
        catch (ErrorCarnetUsuarioNoEncontrado)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (ErrorNoEquiposDisponibles)
        {
            throw; // Re-lanzar excepciones específicas
        }
        catch (Exception ex)
        {            // 3. Interpretar errores específicos del procedimiento insertar_prestamo
            InterpretarErrorCreacionPrestamo(ex, comando);
        }
    }

    private void ValidarEntradaCreacion(CrearPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.CarnetUsuario))
            throw new ErrorNombreRequerido();

        if (comando.GrupoEquipoId == null || comando.GrupoEquipoId.Length == 0)
            throw new ErrorGrupoEquipoIdInvalido();

        if (comando.GrupoEquipoId.Any(id => id <= 0))
            throw new ErrorGrupoEquipoIdInvalido();

        if (comando.FechaPrestamoEsperada == null)
            throw new ErrorFechaPrestamoEsperadaRequerida();

        if (comando.FechaDevolucionEsperada == null)
            throw new ErrorFechaDevolucionEsperadaRequerida();

        if (comando.FechaDevolucionEsperada < comando.FechaPrestamoEsperada)
            throw new ErrorFechaPrestamoYFechaDevolucionInvalidas();
        
        if (comando.Contrato == null)
            throw new System.ArgumentException("El contrato es requerido.");
    }

    private void InterpretarErrorCreacionPrestamo(Exception ex, CrearPrestamoComando comando)
    {
        var errorMessage = ex.Message?.ToLower() ?? "";
        
        // Errores específicos del procedimiento insertar_prestamo
        
        // 1. Errores de préstamo general
        if (errorMessage.Contains("error al crear préstamo general: conflicto de llave única"))
        {
            throw new ErrorRegistroYaExiste();
        }
        else if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear el préstamo general"))
        {
            throw new Exception($"Error inesperado al crear el préstamo general: {ex.Message}", ex);
        }
        else if (errorMessage.Contains("fallo crítico: no se pudo obtener el id del préstamo general creado"))
        {
            throw new Exception("Fallo crítico: No se pudo crear el préstamo", ex);
        }
        
        // 2. Errores de grupos
        else if (errorMessage.Contains("grupo id") && errorMessage.Contains("no existe o está eliminado"))
        {
            throw new ErrorRegistroNoEncontrado();
        }
        
        // 3. Errores de disponibilidad de equipos
        else if (errorMessage.Contains("no se encontró equipo disponible") && errorMessage.Contains("para el grupo id"))
        {
            throw new ErrorNoEquiposDisponibles();
        }
        
        // 4. Errores de detalles de préstamo
        else if (errorMessage.Contains("conflicto de llave única al crear detalle"))
        {
            throw new ErrorRegistroYaExiste();
        }
        else if (errorMessage.Contains("violación de restricción check al crear detalle"))
        {
            throw new Exception($"Error de validación al crear detalle del préstamo: {ex.Message}", ex);
        }
        else if (errorMessage.Contains("violación de llave foránea al crear detalle"))
        {
            throw new ErrorReferenciaInvalida("equipo o préstamo");
        }
        else if (errorMessage.Contains("error inesperado") && errorMessage.Contains("al crear detalle"))
        {
            throw new Exception($"Error inesperado al crear detalle del préstamo: {ex.Message}", ex);
        }
        
        // 5. Errores por códigos SQLSTATE específicos
        else if (ex is ErrorDataBase errorDb)
        {
            // Violación de llave foránea (carnet de usuario no existe)
            if (errorDb.SqlState == "23503")
            {
                if (errorMessage.Contains("carnet") || errorMessage.Contains("usuarios"))
                {
                    throw new ErrorCarnetUsuarioNoEncontrado();
                }
                throw new ErrorReferenciaInvalida("referencia de base de datos");
            }
            
            // Violación de unicidad
            if (errorDb.SqlState == "23505")
            {
                throw new ErrorRegistroYaExiste();
            }
            
            throw new Exception($"Error inesperado de base de datos al crear préstamo: {errorDb.Message}", errorDb);
        }
        
        // 6. Errores de repositorio
        else if (ex is ErrorRepository errorRepo)
        {
            throw new Exception($"Error del repositorio al crear préstamo: {errorRepo.Message}", errorRepo);
        }
        
        // 7. Error genérico
        else
        {
            throw new Exception($"Error inesperado al crear préstamo: {ex.Message}", ex);
        }
    }

    public virtual void EliminarPrestamo(EliminarPrestamoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _prestamoRepository.Eliminar(comando.Id);

            // Sincronizar EstadoEliminado en MongoDB
            var contratosCollection = _mongoDbContexto.Contratos;
            if (contratosCollection != null)
            {
                var filter = Builders<Contrato>.Filter.Eq(c => c.PrestamoId, comando.Id);
                var update = Builders<Contrato>.Update.Set("EstadoEliminado", true);
                contratosCollection.UpdateMany(filter, update);
            }
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Manejo específico para eliminar_prestamo según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";

                // Error: Préstamo no encontrado
                if (mensaje.Contains("no se encontró un préstamo activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }

                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el préstamo"))
                {
                    throw new Exception($"Error inesperado al eliminar préstamo: {errorDb.Message}", errorDb);
                }

                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar préstamo: {errorDb.Message}", errorDb);
            }

            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar préstamo: {errorRepo.Message}", errorRepo);
            }

            throw;
        }
    }

    private void ValidarEntradaEliminacion(EliminarPrestamoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }

    public virtual List<PrestamoDto> ObtenerTodosPrestamos()
    {
        try
        {
            DataTable resultado = _prestamoRepository.ObtenerTodos();
            var lista = new List<PrestamoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila);
                try
                {
                    var contratosCollection = _mongoDbContexto.Contratos;
                    if (contratosCollection != null)
                    {
                        var filter = Builders<Contrato>.Filter.Eq(c => c.PrestamoId, dto.Id);
                        var contrato = contratosCollection.Find(filter).FirstOrDefault();
                        if (contrato != null)
                        {
                            dto.FileId = contrato.FileId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    dto.FileId = null;
                    Console.WriteLine($"Error al consultar MongoDB para el préstamo {dto.Id}: {ex.Message}\n{ex.StackTrace}");
                }
                lista.Add(dto);
            }
            return lista;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general en ObtenerTodosPrestamos: {ex.Message}\n{ex.StackTrace}");
            throw new Exception($"Error al obtener préstamos: {ex.Message}", ex);
        }
    }

    private static PrestamoDto MapearFilaADto(DataRow fila)
    {
        var dto = new PrestamoDto
        {
            Id = Convert.ToInt32(fila["id_prestamo"]),
            CarnetUsuario = fila.Table.Columns.Contains("carnet") && fila["carnet"] != DBNull.Value ? fila["carnet"].ToString() : null,
            NombreUsuario = fila.Table.Columns.Contains("nombre") && fila["nombre"] != DBNull.Value ? fila["nombre"].ToString() : null,
            ApellidoPaternoUsuario = fila.Table.Columns.Contains("apellido_paterno") && fila["apellido_paterno"] != DBNull.Value ? fila["apellido_paterno"].ToString() : null,
            TelefonoUsuario = fila.Table.Columns.Contains("telefono") && fila["telefono"] != DBNull.Value ? fila["telefono"].ToString() : null,
            NombreGrupoEquipo = fila.Table.Columns.Contains("nombre_grupo_equipo") && fila["nombre_grupo_equipo"] != DBNull.Value ? fila["nombre_grupo_equipo"].ToString() : null,
            CodigoImt = fila.Table.Columns.Contains("codigo_imt") && fila["codigo_imt"] != DBNull.Value ? fila["codigo_imt"].ToString() : null,
            FechaSolicitud = fila.Table.Columns.Contains("fecha_solicitud") && fila["fecha_solicitud"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_solicitud"]) : (DateTime?)null,
            FechaPrestamoEsperada = fila.Table.Columns.Contains("fecha_prestamo_esperada") && fila["fecha_prestamo_esperada"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_prestamo_esperada"]) : (DateTime?)null,
            FechaPrestamo = fila.Table.Columns.Contains("fecha_prestamo") && fila["fecha_prestamo"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_prestamo"]) : (DateTime?)null,
            FechaDevolucionEsperada = fila.Table.Columns.Contains("fecha_devolucion_esperada") && fila["fecha_devolucion_esperada"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_devolucion_esperada"]) : (DateTime?)null,
            FechaDevolucion = fila.Table.Columns.Contains("fecha_devolucion") && fila["fecha_devolucion"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_devolucion"]) : (DateTime?)null,
            Observacion = fila.Table.Columns.Contains("observacion") && fila["observacion"] != DBNull.Value ? fila["observacion"].ToString() : null,
            EstadoPrestamo = fila.Table.Columns.Contains("estado_prestamo") && fila["estado_prestamo"] != DBNull.Value ? fila["estado_prestamo"].ToString() : null,
            IdContrato = fila.Table.Columns.Contains("id_contrato") && fila["id_contrato"] != DBNull.Value ? fila["id_contrato"].ToString() : null
        };
        // Si id_contrato es nulo, no lo retornes (deja el valor en null)
        return dto;
    }

    public void AceptarPrestamo(AceptarPrestamoComando comando)
    {
        if (comando.Contrato == null)
            throw new ArgumentException("El contrato es requerido.");

        // Validar que el archivo sea HTML
        if (!comando.Contrato.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("El contrato debe ser un archivo HTML (.html)");

        // Buscar el contrato existente en MongoDB por PrestamoId
        var contratosCollection = _mongoDbContexto.Contratos;
        var filter = Builders<Contrato>.Filter.Eq(c => c.PrestamoId, comando.PrestamoId);
        var contratoExistente = contratosCollection.Find(filter).FirstOrDefault();
        if (contratoExistente == null)
            throw new Exception("No existe un contrato asociado a este préstamo.");

        // Validar que el contrato no esté eliminado
        if (contratoExistente.EstadoEliminado)
            throw new ArgumentException("No se puede aceptar un préstamo eliminado.");

        // Subir el nuevo archivo a GridFS
        string nuevoFileId;
        using (var stream = comando.Contrato.OpenReadStream())
        {
            var objectId = _mongoDbContexto.GestionArchivos.UploadFromStreamAsync(comando.Contrato.FileName, stream).GetAwaiter().GetResult();
            nuevoFileId = objectId.ToString();
        }

        // Actualizar el documento de contrato en MongoDB
        var update = Builders<Contrato>.Update.Set(c => c.FileId, nuevoFileId);
        contratosCollection.UpdateOne(filter, update);


        // Cambiar el estado del préstamo a "activo" en la base de datos relacional
        _prestamoRepository.ActualizarEstado(new ActualizarEstadoPrestamoComando(comando.PrestamoId, "activo"));
    }
}