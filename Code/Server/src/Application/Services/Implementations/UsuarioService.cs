using System.Data;
using Ardalis.Result;

public class UsuarioService : Service<UsuarioDto>, ICrud<UsuarioDto, CrearUsuarioComando, ActualizarUsuarioComando, EliminarUsuarioComando>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository) => _usuarioRepository = usuarioRepository;

    public Result<UsuarioDto?> Crear(CrearUsuarioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<UsuarioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var idCarrera = _usuarioRepository.ObtenerCarreraIdPorNombre(comando.NombreCarrera!);
        if (idCarrera == null)
            return Result<UsuarioDto?>.NotFound("La carrera no fue encontrada");

        var result = _usuarioRepository.Crear(idCarrera.Value, comando);
        return result;
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _usuarioRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener los usuarios");
        return result;
    }

    public Result<UsuarioDto?> Actualizar(ActualizarUsuarioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<UsuarioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_usuarioRepository.ExisteActivoPorCarnet(comando.Carnet!))
            return Result<UsuarioDto?>.NotFound("El usuario no fue encontrado");

        int? idCarrera = null;
        if (!string.IsNullOrWhiteSpace(comando.NombreCarrera))
        {
            idCarrera = _usuarioRepository.ObtenerCarreraIdPorNombre(comando.NombreCarrera);
            if (idCarrera == null)
                return Result<UsuarioDto?>.NotFound("La carrera no fue encontrada");
        }

        if (!string.IsNullOrWhiteSpace(comando.Rol))
        {
            if (comando.Rol != "administrador" && comando.Rol != "estudiante")
                return Result<UsuarioDto?>.Invalid(new ValidationError("Rol", "El rol no es válido"));
        }

        var result = _usuarioRepository.Actualizar(idCarrera, comando);
        return result;
    }

    public Result<UsuarioDto?> Eliminar(EliminarUsuarioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<UsuarioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_usuarioRepository.ExisteActivoPorCarnet(comando.Carnet!))
            return Result<UsuarioDto?>.NotFound("El usuario no fue encontrado");

        var result = _usuarioRepository.Eliminar(comando);
        return result;
    }

    public UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta)
    {
        try
        {
            DataTable? resultado = _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
            if (resultado?.Rows.Count > 0) return MapearFilaADto(resultado.Rows[0]) as UsuarioDto;
            return null;
        }
        catch { throw; }
    }

    private Result<CrearUsuarioComando> ValidarEntrada(CrearUsuarioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Carnet))
            errors.Add(new("Carnet", "El carnet es requerido"));

        if (comando?.Carnet?.Length > 15)
            errors.Add(new("Carnet", "El carnet no puede tener más de 15 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Nombre?.Length > 100)
            errors.Add(new("Nombre", "El nombre no puede tener más de 100 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.ApellidoPaterno))
            errors.Add(new("ApellidoPaterno", "El apellido paterno es requerido"));

        if (comando?.ApellidoPaterno?.Length > 100)
            errors.Add(new("ApellidoPaterno", "El apellido paterno no puede tener más de 100 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.ApellidoMaterno))
            errors.Add(new("ApellidoMaterno", "El apellido materno es requerido"));

        if (comando?.ApellidoMaterno?.Length > 100)
            errors.Add(new("ApellidoMaterno", "El apellido materno no puede tener más de 100 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.Email) || !IsValidEmail(comando?.Email ?? string.Empty))
            errors.Add(new("Email", "El email es inválido"));

        if (comando?.Email?.Length > 150)
            errors.Add(new("Email", "El email no puede tener más de 150 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.Contrasena))
            errors.Add(new("Contrasena", "La contraseña es requerida"));

        if (comando?.Contrasena?.Length < 6)
            errors.Add(new("Contrasena", "La contraseña debe tener al menos 6 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.NombreCarrera))
            errors.Add(new("NombreCarrera", "La carrera es requerida"));

        if (string.IsNullOrWhiteSpace(comando?.Telefono))
            errors.Add(new("Telefono", "El teléfono es requerido"));

        if (comando?.Telefono?.Length > 20)
            errors.Add(new("Telefono", "El teléfono no puede tener más de 20 caracteres"));

        if (comando?.Rol != "administrador" && comando?.Rol != "estudiante")
            errors.Add(new("Rol", "El rol no es válido"));

        return errors.Any()
            ? Result<CrearUsuarioComando>.Invalid(errors.ToArray())
            : Result<CrearUsuarioComando>.Success(comando!);
    }

    private Result<ActualizarUsuarioComando> ValidarEntrada(ActualizarUsuarioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (!string.IsNullOrWhiteSpace(comando?.Carnet) && comando.Carnet.Length > 15)
            errors.Add(new("Carnet", "El carnet no puede tener más de 15 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.ApellidoPaterno) && comando.ApellidoPaterno.Length > 255)
            errors.Add(new("ApellidoPaterno", "El apellido paterno no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.ApellidoMaterno) && comando.ApellidoMaterno.Length > 255)
            errors.Add(new("ApellidoMaterno", "El apellido materno no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Email) && comando.Email.Length > 150)
            errors.Add(new("Email", "El email no puede tener más de 150 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Email) && !IsValidEmail(comando.Email))
            errors.Add(new("Email", "El email es inválido"));

        if (!string.IsNullOrWhiteSpace(comando?.Contrasena) && comando.Contrasena.Length < 6)
            errors.Add(new("Contrasena", "La contraseña debe tener al menos 6 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Telefono) && comando.Telefono.Length > 20)
            errors.Add(new("Telefono", "El teléfono no puede tener más de 20 caracteres"));

        return errors.Any()
            ? Result<ActualizarUsuarioComando>.Invalid(errors.ToArray())
            : Result<ActualizarUsuarioComando>.Success(comando!);
    }

    private Result<EliminarUsuarioComando> ValidarEntrada(EliminarUsuarioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Carnet))
            errors.Add(new("Carnet", "El carnet es requerido"));

        if (comando?.Carnet?.Length > 15)
            errors.Add(new("Carnet", "El carnet no puede tener más de 15 caracteres"));

        return errors.Any()
            ? Result<EliminarUsuarioComando>.Invalid(errors.ToArray())
            : Result<EliminarUsuarioComando>.Success(comando!);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
        catch { return false; }
    }

    protected override Dto MapearFilaADto(DataRow fila)
    {
        return new UsuarioDto
        {
            Carnet = fila["carnet"] == DBNull.Value ? null : fila["carnet"].ToString(),
            Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
            ApellidoPaterno = fila["apellido_paterno"] == DBNull.Value ? null : fila["apellido_paterno"].ToString(),
            ApellidoMaterno = fila["apellido_materno"] == DBNull.Value ? null : fila["apellido_materno"].ToString(),
            CarreraNombre = fila["carrera"] == DBNull.Value ? null : fila["carrera"].ToString(),
            Rol = fila["rol"] == DBNull.Value ? null : fila["rol"].ToString(),
            Email = fila["email"] == DBNull.Value ? null : fila["email"].ToString(),
            Telefono = fila["telefono"] == DBNull.Value ? null : fila["telefono"].ToString(),
            TelefonoReferencia = fila["telefono_referencia"] == DBNull.Value ? null : fila["telefono_referencia"].ToString(),
            NombreReferencia = fila["nombre_referencia"] == DBNull.Value ? null : fila["nombre_referencia"].ToString(),
            EmailReferencia = fila["email_referencia"] == DBNull.Value ? null : fila["email_referencia"].ToString()
        };
    }
}
