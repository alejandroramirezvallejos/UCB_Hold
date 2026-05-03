using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    public UsuarioService(UsuarioRepository repository) : base(repository) { }

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
            return Result<UsuarioDto>.NotFound();
        
        return Result<UsuarioDto>.Success(MapToDto(usuario));
    }

    private UsuarioDto MapToDto(UsuarioEntity entity) => new()
    {
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Rol = entity.Rol,
        Telefono = entity.Telefono,
        TelefonoReferencia = entity.TelefonoReferencia,
        NombreReferencia = entity.NombreReferencia
    };
}
