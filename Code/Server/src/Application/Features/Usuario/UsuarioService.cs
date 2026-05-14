using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private readonly ApplicationDbContext _dbContext;

    public UsuarioService(UsuarioRepository repository, ApplicationDbContext dbContext)
        : base(repository) => _dbContext = dbContext;

    public override async Task<Result<UsuarioDto>> Create(UsuarioEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Email) || !entity.Email.Contains("@"))
            return Result<UsuarioDto>.Error("Email inválido");

        var carnetExists = await _dbContext.Usuarios
            .IgnoreQueryFilters()
            .AnyAsync(usuario => usuario.Carnet == entity.Carnet);

        if (carnetExists)
            return Result<UsuarioDto>.Error("Carnet ya existe");

        var emailExists = await _dbContext.Usuarios
            .IgnoreQueryFilters()
            .AnyAsync(usuario => usuario.Email == entity.Email);

        if (emailExists)
            return Result<UsuarioDto>.Error("Email ya existe");

        var carreraExists = await _dbContext.Carreras
            .AnyAsync(carrera => carrera.Id == entity.IdCarrera && !carrera.EstadoEliminado);

        if (!carreraExists)
            return Result<UsuarioDto>.Error("Carrera no existe");

        entity.Contrasena = BCrypt.Net.BCrypt.HashPassword(entity.Contrasena);

        var result = await base.Create(entity);
        
        if (result.IsSuccess && result.Value != null)
        {
            var carreraResult = await _dbContext.Carreras.AsNoTracking().FirstOrDefaultAsync(carrera => carrera.Id == entity.IdCarrera);
            if (carreraResult != null)
                result.Value.CarreraNombre = carreraResult.Nombre;
        }
        
        return result;
    }

    public async Task<Result<UsuarioDto>> CreateFromDto(UsuarioDto dto)
    {
        var idCarrera = dto.IdCarrera ?? 0;
        
        if (idCarrera == 0 && !string.IsNullOrWhiteSpace(dto.CarreraNombre))
        {
            var carreraPorNombre = await _dbContext.Carreras
                .AsNoTracking()
                .FirstOrDefaultAsync(carrera => carrera.Nombre == dto.CarreraNombre && !carrera.EstadoEliminado);
            
            if (carreraPorNombre != null)
                idCarrera = carreraPorNombre.Id;
        }

        var entity = new UsuarioEntity
        {
            Carnet = dto.Carnet ?? string.Empty,
            Nombre = dto.Nombre ?? string.Empty,
            ApellidoPaterno = dto.ApellidoPaterno ?? string.Empty,
            ApellidoMaterno = dto.ApellidoMaterno ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Contrasena = dto.Contrasena ?? string.Empty,
            Telefono = dto.Telefono ?? string.Empty,
            TelefonoReferencia = dto.TelefonoReferencia,
            NombreReferencia = dto.NombreReferencia,
            EmailReferencia = dto.EmailReferencia,
            IdCarrera = idCarrera,
            Rol = dto.Rol?.ToLowerInvariant() switch
            {
                "docente" => Core.Entities.TipoUsuario.Docente,
                "administrador" => Core.Entities.TipoUsuario.Administrador,
                _ => Core.Entities.TipoUsuario.Estudiante
            }
        };

        return await Create(entity);
    }

    public async Task<Result<UsuarioDto>> UpdateFromDto(string carnet, UsuarioDto dto)
    {
        var existing = await _dbContext.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Carnet == carnet && !usuario.EstadoEliminado);

        if (existing == null)
            return Result<UsuarioDto>.NotFound();

        var idCarrera = dto.IdCarrera ?? 0;
        
        if (idCarrera == 0 && !string.IsNullOrWhiteSpace(dto.CarreraNombre))
        {
            var carreraPorNombre = await _dbContext.Carreras
                .AsNoTracking()
                .FirstOrDefaultAsync(carrera => carrera.Nombre == dto.CarreraNombre && !carrera.EstadoEliminado);
            
            if (carreraPorNombre != null)
                idCarrera = carreraPorNombre.Id;
        }

        existing.Nombre = dto.Nombre ?? existing.Nombre;
        existing.ApellidoPaterno = dto.ApellidoPaterno ?? existing.ApellidoPaterno;
        existing.ApellidoMaterno = dto.ApellidoMaterno ?? existing.ApellidoMaterno;
        existing.Email = dto.Email ?? existing.Email;
        existing.Telefono = dto.Telefono ?? existing.Telefono;
        existing.TelefonoReferencia = dto.TelefonoReferencia ?? existing.TelefonoReferencia;
        existing.NombreReferencia = dto.NombreReferencia ?? existing.NombreReferencia;
        existing.EmailReferencia = dto.EmailReferencia ?? existing.EmailReferencia;
        
        if (idCarrera > 0) 
            existing.IdCarrera = idCarrera;
        
        existing.Rol = dto.Rol?.ToLowerInvariant() switch
        {
            "docente" => Core.Entities.TipoUsuario.Docente,
            "administrador" => Core.Entities.TipoUsuario.Administrador,
            _ => existing.Rol
        };
        
        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            existing.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

        await _dbContext.SaveChangesAsync();
        var resultDto = MapToDto(existing);
        var carreraFinal = await _dbContext.Carreras.AsNoTracking().FirstOrDefaultAsync(carrera => carrera.Id == existing.IdCarrera);
        
        if (carreraFinal != null)
            resultDto.CarreraNombre = carreraFinal.Nombre;

        return Result<UsuarioDto>.Success(resultDto);
    }

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var usuario = await Repository.GetByCarnet(carnet);

        if (usuario == null)
            return Result<UsuarioDto>.NotFound();

        var dto = MapToDto(usuario);
        var carrera = await _dbContext.Carreras.AsNoTracking().FirstOrDefaultAsync(carrera => carrera.Id == usuario.IdCarrera);
        
        if (carrera != null)
            dto.CarreraNombre = carrera.Nombre;

        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<UsuarioDto>> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<UsuarioDto>.Unauthorized("Credenciales requeridas");

        var loginData = await _dbContext.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.Email == email && !usuario.EstadoEliminado)
            .Select(usuario => new
            {
                Usuario = usuario,
                CarreraNombre = _dbContext.Carreras
                    .Where(carrera => carrera.Id == usuario.IdCarrera)
                    .Select(carrera => carrera.Nombre)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (loginData == null)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var usuario = loginData.Usuario;
        bool passwordValid = !string.IsNullOrEmpty(usuario.Contrasena)
                          && usuario.Contrasena.StartsWith("$2")
                          && usuario.Contrasena.Length == 60
                          && BCrypt.Net.BCrypt.Verify(password, usuario.Contrasena);

        if (!passwordValid)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var dto = MapToDto(usuario);
        dto.CarreraNombre = loginData.CarreraNombre;

        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<object>> Delete(string carnet)
        => await Repository.Delete(carnet);

    private UsuarioDto MapToDto(UsuarioEntity entity) => new()
    {
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Rol = entity.Rol.ToString().ToLowerInvariant(),
        Email = entity.Email,
        IdCarrera = entity.IdCarrera,
        CarreraNombre = null,
        Telefono = entity.Telefono,
        TelefonoReferencia = entity.TelefonoReferencia,
        NombreReferencia = entity.NombreReferencia,
        EmailReferencia = entity.EmailReferencia,
        ImagenFrenteCarnet = entity.ImagenFrenteCarnet,
        ImagenAtrasCarnet = entity.ImagenAtrasCarnet
    };
}
