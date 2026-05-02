using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService
{
    private readonly UsuarioRepository _repository;

    public UsuarioService(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UsuarioDetailDto>> Create(UsuarioEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<UsuarioDetailDto>.Error("Error al crear usuario")
            : Result<UsuarioDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<UsuarioDetailDto>> Update(UsuarioEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<UsuarioDetailDto>.Error("Error al actualizar usuario")
            : Result<UsuarioDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        var result = await _repository.Delete(carnet);
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar usuario");
    }

    public async Task<Result<UsuarioDetailDto>> Get(string carnet)
    {
        var usuario = await _repository.GetByCarnet(carnet);
        if (usuario == null)
            return Result<UsuarioDetailDto>.NotFound();

        var detailDto = MapEntityToDetailDto(usuario);
        return Result<UsuarioDetailDto>.Success(detailDto);
    }

    public async Task<Result<List<UsuarioListDto>>> GetAllUsers()
    {
        var result = await _repository.GetAll(null);
        return result.IsSuccess
            ? Result<List<UsuarioListDto>>.Success(result.Value)
            : Result<List<UsuarioListDto>>.Error("Error al obtener usuarios");
    }

    public async Task<Result<UsuarioDetailDto>> InitiateSession(string email, string password)
    {
        var usuario = await _repository.GetByEmail(email);
        if (usuario == null)
            return Result<UsuarioDetailDto>.NotFound();

        return usuario.Contrasena != password
            ? Result<UsuarioDetailDto>.Error("Credenciales inválidas")
            : Result<UsuarioDetailDto>.Success(MapEntityToDetailDto(usuario));
    }

    private static UsuarioDetailDto MapEntityToDetailDto(UsuarioEntity entity) => new()
    {
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Email = entity.Email,
        IdCarrera = entity.IdCarrera,
        Rol = entity.Rol,
        Telefono = entity.Telefono,
        TelefonoReferencia = entity.TelefonoReferencia,
        NombreReferencia = entity.NombreReferencia,
        EmailReferencia = entity.EmailReferencia
    };

    private static UsuarioDetailDto MapListDtoToDetailDto(UsuarioListDto dto) => new()
    {
        Carnet = dto.Carnet,
        Nombre = dto.Nombre,
        ApellidoPaterno = dto.ApellidoPaterno,
        ApellidoMaterno = dto.ApellidoMaterno,
        Email = dto.Email,
        IdCarrera = dto.IdCarrera,
        Rol = dto.Rol,
        Telefono = dto.Telefono,
        TelefonoReferencia = dto.TelefonoReferencia,
        NombreReferencia = dto.NombreReferencia,
        EmailReferencia = dto.EmailReferencia
    };
}

