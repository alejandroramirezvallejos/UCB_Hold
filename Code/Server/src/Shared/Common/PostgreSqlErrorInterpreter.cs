using Npgsql;
using Shared.Common;

namespace Shared.Common
{
    public static class PostgreSqlErrorCodes
    {
        public const string UniqueViolation = "23505";           // Duplicate key
        public const string ForeignKeyViolation = "23503";      // Foreign key constraint
        public const string CheckViolation = "23514";           // Check constraint
        public const string NotNullViolation = "23502";         // NOT NULL constraint
        public const string UndefinedFunction = "42883";        // Function/procedure does not exist
        public const string UndefinedColumn = "42703";          // Column does not exist
        public const string UndefinedTable = "42P01";           // Table does not exist
        public const string SyntaxError = "42601";              // Syntax error
        public const string RaiseException = "P0001";           // Custom RAISE EXCEPTION
    }
}

namespace Shared.Common
{
    public static class PostgreSqlErrorInterpreter
    {
        public static Exception InterpretarError(Exception ex, string operacion, string entidad, Dictionary<string, object?>? parametros = null)
        {
            // Si es NpgsqlException, extraer información específica
            if (ex is NpgsqlException pgEx)
            {
                return InterpretarNpgsqlException(pgEx, operacion, entidad, parametros);
            }
            
            // Si es Exception con mensaje que contiene info de PostgreSQL
            var errorMessage = ex.Message.ToLower();
            
            if (errorMessage.Contains("23505") || errorMessage.Contains("duplicate key"))
            {
                return new ErrorRegistroYaExiste();
            }
            
            if (errorMessage.Contains("23503"))
            {
                if (errorMessage.Contains("is not present") || errorMessage.Contains("does not exist"))
                {
                    return new ErrorReferenciaInvalida(entidad);
                }
            }
            
            if (errorMessage.Contains("23502") || errorMessage.Contains("not null"))
            {
                return new ErrorNombreRequerido();
            }
            
            if (errorMessage.Contains("42883") || errorMessage.Contains("does not exist"))
            {
                return new Exception("Procedimiento almacenado no encontrado");
            }
            
            if (errorMessage.Contains("p0001"))
            {
                return InterpretarErrorPersonalizado(errorMessage, entidad, parametros);
            }
            
            return new Exception($"Error al {operacion} {entidad}: {ex.Message}", ex);
        }

        private static Exception InterpretarNpgsqlException(NpgsqlException pgEx, string operacion, string entidad, Dictionary<string, object?>? parametros)
        {            return pgEx.SqlState switch
            {
                PostgreSqlErrorCodes.UniqueViolation => InterpretarUniqueViolation(pgEx, entidad, parametros),
                PostgreSqlErrorCodes.ForeignKeyViolation => InterpretarForeignKeyViolation(pgEx, entidad, parametros),
                PostgreSqlErrorCodes.CheckViolation => new Exception($"Los datos no cumplen las restricciones: {pgEx.Message}"),
                PostgreSqlErrorCodes.NotNullViolation => new ErrorNombreRequerido(),
                PostgreSqlErrorCodes.UndefinedFunction => new Exception($"Procedimiento almacenado no encontrado: {pgEx.Message}"),
                PostgreSqlErrorCodes.RaiseException => InterpretarErrorPersonalizado(pgEx.Message?.ToLower() ?? "", entidad, parametros),
                _ => new Exception($"Error PostgreSQL ({pgEx.SqlState}): {pgEx.Message}")
            };
        }        private static Exception InterpretarUniqueViolation(NpgsqlException pgEx, string entidad, Dictionary<string, object?>? parametros)
        {
            var errorMessage = pgEx.Message?.ToLower() ?? "";
            var errorDetail = pgEx.Message?.ToLower() ?? "";
            
            // Componentes
            if (errorMessage.Contains("componentes_nombre_key") || errorDetail.Contains("nombre") ||
                errorMessage.Contains("componentes_codigo_imt_key") || errorDetail.Contains("codigo_imt"))
            {
                return new ErrorComponenteYaExiste();
            }
            
            // Usuarios y otros registros duplicados
            return new ErrorRegistroYaExiste();
        }        private static Exception InterpretarForeignKeyViolation(NpgsqlException pgEx, string entidad, Dictionary<string, object?>? parametros)
        {
            var errorMessage = pgEx.Message?.ToLower() ?? "";
            var errorDetail = pgEx.Message?.ToLower() ?? "";
            
            // Referencia que no existe
            if (errorDetail.Contains("is not present") || errorDetail.Contains("does not exist"))
            {
                if (errorDetail.Contains("carnet"))
                {
                    return new ErrorCarnetUsuarioNoEncontrado();
                }
                
                return new ErrorReferenciaInvalida(entidad);
            }
            
            return new ErrorReferenciaInvalida(entidad);
        }
        private static Exception InterpretarErrorPersonalizado(string errorMessage, string entidad, Dictionary<string, object?>? parametros)
        {
            // Errores específicos de procedimientos almacenados
            if (errorMessage.Contains("no existe equipo") || errorMessage.Contains("equipo no encontrado"))
            {
                return new ErrorRegistroNoEncontrado();
            }

            if (errorMessage.Contains("no hay equipos disponibles") || errorMessage.Contains("no se encontró equipo disponible"))
            {
                return new ErrorNoEquiposDisponibles();
            }

            if (errorMessage.Contains("usuario no encontrado") || errorMessage.Contains("carnet") && errorMessage.Contains("no existe"))
            {
                var carnet = parametros?.GetValueOrDefault("carnetUsuario")?.ToString();
                return new ErrorCarnetUsuarioNoEncontrado();
            }

            if (errorMessage.Contains("grupo id") && errorMessage.Contains("no existe"))
            {
                return new ErrorRegistroNoEncontrado();
            }

            return new Exception($"Error personalizado: {errorMessage}");
        }
    }
}
