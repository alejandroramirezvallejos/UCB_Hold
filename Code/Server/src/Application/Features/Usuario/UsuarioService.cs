using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService
{
    private readonly UsuarioRepository _repository;

    public UsuarioService(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UsuarioDto>> Create(UsuarioEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<UsuarioDto>> Update(UsuarioEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(string carnet)
        => await _repository.Delete(carnet);

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var usuario = await _repository.GetByCarnet(carnet);
        return usuario == null
            ? Result<UsuarioDto>.NotFound()
            : Result<UsuarioDto>.Success(MapToDto(usuario));
    }

    public async Task<Result<List<UsuarioDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);

    public async Task<Result<List<UsuarioDto>>> GetAllUsers()
        => await _repository.GetAll(null);

    public async Task<Result<UsuarioDto>> InitiateSession(string email, string password)
    {
        var usuario = await _repository.GetByEmail(email);
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
