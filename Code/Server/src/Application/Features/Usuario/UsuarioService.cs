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

    public async Task<Result<UsuarioDetail>> Create(UsuarioEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<UsuarioDetail>.Error("Error al crear usuario")
            : Result<UsuarioDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<UsuarioDetail>> Update(UsuarioEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<UsuarioDetail>.Error("Error al actualizar usuario")
            : Result<UsuarioDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        var result = await _repository.Delete(carnet);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar usuario");
    }

    public async Task<Result<UsuarioDetail>> Get(string carnet)
    {
        var usuario = await _repository.GetByCarnet(carnet);
        
        if (usuario == null)
            return Result<UsuarioDetail>.NotFound();

        var detailDto = MapEntityToDetail(usuario);
        
        return Result<UsuarioDetail>.Success(detailDto);
    }

    public async Task<Result<List<UsuarioList>>> GetAllUsers()
    {
        var result = await _repository.GetAll();
        
        return result.IsSuccess
            ? Result<List<UsuarioList>>.Success(result.Value)
            : Result<List<UsuarioList>>.Error("Error al obtener usuarios");
    }

    public async Task<Result<UsuarioDetail>> InitiateSession(string email, string password)
    {
        var usuario = await _repository.GetByEmail(email);
        
        if (usuario == null)
            return Result<UsuarioDetail>.NotFound();

        return usuario.Contrasena != password
            ? Result<UsuarioDetail>.Error("Credenciales inválidas")
            : Result<UsuarioDetail>.Success(MapEntityToDetail(usuario));
    }

    private static UsuarioDetail MapEntityToDetail(UsuarioEntity entity) => new()
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

    private static UsuarioDetail MapListToDetail(UsuarioList dto) => new()
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

