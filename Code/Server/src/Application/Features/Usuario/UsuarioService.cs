using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private readonly UsuarioRepository _repository;
    private readonly ApplicationDbContext _dbContext;

    public UsuarioService(UsuarioRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _repository = repository;
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

    public async Task<Result<List<UsuarioDto>>> GetAllUsers()
        => await Repository.GetAll();

    public async Task<Result<UsuarioDto>> InitiateSession(string email, string password)
    {
        var usuario = await Repository.GetByEmail(email);

        if (usuario == null)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        if (!BCrypt.Net.BCrypt.Verify(password, usuario.Contrasena))
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        return Result<UsuarioDto>.Success(MapToDto(usuario));
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
