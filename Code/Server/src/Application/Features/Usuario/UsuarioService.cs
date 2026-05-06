using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private readonly ApplicationDbContext _dbContext;

    public UsuarioService(UsuarioRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _dbContext = dbContext;
    }

    public override async Task<Result<UsuarioDto>> Create(UsuarioEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Email) || !entity.Email.Contains("@"))
            return Result<UsuarioDto>.Error("Email inválido");

        var carnetExists = await _dbContext.Usuarios
            .AnyAsync(u => u.Carnet == entity.Carnet && !u.EstadoEliminado);

        if (carnetExists)
            return Result<UsuarioDto>.Error("Carnet ya existe");

        var emailExists = await _dbContext.Usuarios
            .AnyAsync(u => u.Email == entity.Email && !u.EstadoEliminado);

        if (emailExists)
            return Result<UsuarioDto>.Error("Email ya existe");

        var carreraExists = await _dbContext.Carreras
            .AnyAsync(c => c.Id == entity.IdCarrera && !c.EstadoEliminado);
        
        if (!carreraExists)
            return Result<UsuarioDto>.Error("Carrera no existe");

        entity.Contrasena = BCrypt.Net.BCrypt.HashPassword(entity.Contrasena);

        return await base.Create(entity);
    }

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var usuario = await Repository.GetByCarnet(carnet);

        return usuario == null
            ? Result<UsuarioDto>.NotFound()
            : Result<UsuarioDto>.Success(MapToDto(usuario));
    }

    public async Task<Result<List<UsuarioDto>>> GetAll()
        => await Repository.GetAll();

    public async Task<Result<UsuarioDto>> Login(string email, string password)
    {
        var usuario = await _dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && !u.EstadoEliminado);

        if (usuario == null)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        bool passwordValid;

        if (string.IsNullOrEmpty(usuario.Contrasena))
            passwordValid = false;
        else if (usuario.Contrasena.StartsWith("$2") && usuario.Contrasena.Length == 60)
            passwordValid = BCrypt.Net.BCrypt.Verify(password, usuario.Contrasena);
        else
            passwordValid = password == usuario.Contrasena;

        return passwordValid
            ? Result<UsuarioDto>.Success(MapToDto(usuario))
            : Result<UsuarioDto>.Unauthorized("Credenciales inválidas");
    }

    public async Task<Result<object>> Delete(string carnet)
        => await Repository.Delete(carnet);

    private UsuarioDto MapToDto(UsuarioEntity entity) => new()
    {
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Rol = entity.Rol,
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
