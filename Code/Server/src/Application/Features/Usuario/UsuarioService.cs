using Ardalis.Result;
using BCryptLib = BCrypt.Net.BCrypt;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private readonly UsuarioMapper _mapper;

    public UsuarioService(UsuarioRepository repository, UsuarioMapper mapper, IValidator<UsuarioDto> validator)
        : base(repository, validator, mapper) =>
        _mapper = mapper;

    public override async Task<Result<UsuarioDto>> Create(UsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Contrasena))
            return Result<UsuarioDto>.Error("Contraseña requerida");

        await ResolveCarrera(dto);

        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<UsuarioDto>();

        if (await Repository.ExistsByCarnet(dto.Carnet!))
            return Result<UsuarioDto>.Error("Carnet ya existe");

        if (await Repository.ExistsByEmail(dto.Email!))
            return Result<UsuarioDto>.Error("Email ya existe");

        if (!string.IsNullOrWhiteSpace(dto.Telefono) && await Repository.ExistsByTelefono(dto.Telefono))
            return Result<UsuarioDto>.Error("Teléfono ya registrado");

        var entity = MapToEntity(dto);
        entity.Contrasena = BCryptLib.HashPassword(dto.Contrasena, workFactor: 10);
        var result = await CreateEntity(entity);

        if (result.IsSuccess && result.Value != null)
            result.Value.CarreraNombre = await Repository.GetCarreraName(entity.IdCarrera);

        return result;
    }

    public async Task<Result<UsuarioDto>> Update(string carnet, UsuarioDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<UsuarioDto>();

        var existing = await Repository.GetTrackedByCarnet(carnet);

        if (existing == null)
            return Result<UsuarioDto>.NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Telefono) && await Repository.ExistsByTelefono(dto.Telefono, carnet))
            return Result<UsuarioDto>.Error("Teléfono ya registrado");

        await ResolveCarrera(dto);
        _mapper.Update(dto, existing);

        if ((dto.IdCarrera ?? 0) > 0)
            existing.IdCarrera = dto.IdCarrera!.Value;

        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            existing.Contrasena = BCryptLib.HashPassword(dto.Contrasena, workFactor: 10);

        await Repository.UpdateEntity(existing);

        var resultDto = _mapper.ToDto(existing);
        resultDto.CarreraNombre = await Repository.GetCarreraName(existing.IdCarrera);

        return Result<UsuarioDto>.Success(resultDto);
    }

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var usuario = await Repository.GetByCarnet(carnet);

        if (usuario == null)
            return Result<UsuarioDto>.NotFound();

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = await Repository.GetCarreraName(usuario.IdCarrera);

        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<UsuarioDto>> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<UsuarioDto>.Unauthorized("Credenciales requeridas");

        var (usuario, carreraNombre) = await Repository.GetByEmailWithCarrera(email);

        if (usuario == null)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var passwordValid = !string.IsNullOrWhiteSpace(usuario.Contrasena)
                         && await Task.Run(() => BCryptLib.Verify(password, usuario.Contrasena));

        if (!passwordValid)
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = carreraNombre;

        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<object>> Delete(string carnet) => await Repository.Delete(carnet);

    private async Task ResolveCarrera(UsuarioDto dto)
    {
        if ((dto.IdCarrera ?? 0) > 0)
            return;

        if (string.IsNullOrWhiteSpace(dto.CarreraNombre))
            return;

        dto.IdCarrera = await Repository.FindCarreraIdByName(dto.CarreraNombre);
    }
}
