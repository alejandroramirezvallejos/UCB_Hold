using Ardalis.Result;

namespace IMT_Reservas.Server.Core.Errors;

public static class ErrorFactory
{
    public static ValidationError NotFound<T>() where T : class
        => new ValidationError(GetEntityName<T>(), $"El {GetEntityName<T>()} no fue encontrado");

    public static ValidationError AlreadyExists<T>() where T : class
        => new ValidationError(GetEntityName<T>(), $"Ya existe {GetArticle<T>()} {GetEntityName<T>()} activo con estos datos");

    public static ValidationError RequiredField(string fieldName)
        => new ValidationError(fieldName, $"El {fieldName} es requerido");

    public static ValidationError InvalidField<T>(string fieldName) where T : class
        => new ValidationError(fieldName, $"El {fieldName} es inválido");

    public static ValidationError InvalidValue<T>(string fieldName, string reason) where T : class
        => new ValidationError(fieldName, $"El {fieldName} {reason}");

    public static ValidationError ConflictError<T>(string reason) where T : class
        => new ValidationError(GetEntityName<T>(), $"Conflicto en {GetEntityName<T>()}: {reason}");

    public static ValidationError DatabaseError(string operation)
        => new ValidationError("Database", $"Error de base de datos durante {operation}");

    public static ValidationError UnauthorizedError()
        => new ValidationError("Authorization", "No autorizado para realizar esta acción");

    public static ValidationError ForbiddenError()
        => new ValidationError("Authorization", "Acceso denegado");

    public static ValidationError UnexpectedError()
        => new ValidationError("System", "Error inesperado del sistema");

    private static string GetEntityName<T>() where T : class
        => typeof(T).Name switch
        {
            "Prestamo" => "préstamo",
            "Usuario" => "usuario",
            "Carrera" => "carrera",
            "Categoria" => "categoría",
            "Equipo" => "equipo",
            "GrupoEquipo" => "grupo de equipo",
            "Gavetero" => "gavetero",
            "Mueble" => "mueble",
            "Accesorio" => "accesorio",
            "Componente" => "componente",
            "EmpresaMantenimiento" => "empresa de mantenimiento",
            "Mantenimiento" => "mantenimiento",
            "Comentario" => "comentario",
            "Notificacion" => "notificación",
            "ComentarioLike" => "like",
            _ => typeof(T).Name.ToLower()
        };

    private static string GetArticle<T>() where T : class
        => GetEntityName<T>() switch
        {
            var x when x[0] is 'a' or 'e' or 'i' or 'o' or 'u' => "un",
            _ => "un"
        };
}
