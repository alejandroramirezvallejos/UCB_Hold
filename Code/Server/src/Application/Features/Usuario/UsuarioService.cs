using Ardalis.Result;
using BCryptLib = BCrypt.Net.BCrypt;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Cache;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Application.Features.Jwt;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.Extensions.Options;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private static readonly TimeSpan UsuarioCacheTtl = TimeSpan.FromMinutes(30);
    private readonly UsuarioMapper   _mapper;
    private readonly JwtService      _jwtService;
    private readonly JwtSettings     _jwtSettings;
    private readonly CacheRepository _cacheRepository;

    public UsuarioService(UsuarioRepository repository,
        UsuarioMapper mapper, IValidator<UsuarioDto> validator,
        JwtService jwtService, IOptions<JwtSettings> jwtSettings,
        CacheRepository cacheRepository, AuditLogService audit)
        : base(repository, validator, mapper, audit)
    {
        _mapper          = mapper;
        _jwtService      = jwtService;
        _jwtSettings     = jwtSettings.Value;
        _cacheRepository = cacheRepository;
    }

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
        entity.Contrasena = BCryptLib.HashPassword(dto.Contrasena, workFactor: 12);
        var result = await CreateEntity(entity);

        if (result.IsSuccess && result.Value != null)
        {
            result.Value.CarreraNombre = await Repository.GetCarreraName(entity.IdCarrera);
            await Audit!.Log(AuditAccion.Crear, typeof(UsuarioEntity).Name, entity.Carnet);
        }

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
            existing.Contrasena = BCryptLib.HashPassword(dto.Contrasena, workFactor: 12);

        await Repository.UpdateEntity(existing);

        var resultDto = _mapper.ToDto(existing);
        resultDto.CarreraNombre = await Repository.GetCarreraName(existing.IdCarrera);

        _ = await _cacheRepository.Remove(CacheKeys.Usuario(carnet));
        await Audit!.Log(AuditAccion.Editar, typeof(UsuarioEntity).Name, carnet);

        return Result<UsuarioDto>.Success(resultDto);
    }

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var cacheKey = CacheKeys.Usuario(carnet);
        var cacheResult = await _cacheRepository.Get<UsuarioDto>(cacheKey);
        
        if (cacheResult.IsSuccess) 
            return Result<UsuarioDto>.Success(cacheResult.Value);

        var usuario = await Repository.GetByCarnet(carnet);

        if (usuario == null)
            return Result<UsuarioDto>.NotFound();

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = await Repository.GetCarreraName(usuario.IdCarrera);

        _ = await _cacheRepository.Set(cacheKey, dto, UsuarioCacheTtl);

        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<LoginDto>> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<LoginDto>.Unauthorized("Credenciales requeridas");

        var (usuario, carreraNombre) = await Repository.GetByEmailWithCarrera(email);

        if (usuario == null)
            return Result<LoginDto>.Unauthorized("Credenciales inválidas");

        var passwordValid = !string.IsNullOrWhiteSpace(usuario.Contrasena)
                         && BCryptLib.Verify(password, usuario.Contrasena);

        if (!passwordValid)
            return Result<LoginDto>.Unauthorized("Credenciales inválidas");

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = carreraNombre;

        var accessToken  = _jwtService.GenerateAccessToken(dto);
        var refreshToken = JwtService.GenerateRefreshToken();

        await Repository.UpdateRefreshToken(
            usuario.Carnet!,
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        return Result<LoginDto>.Success(new LoginDto
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            Usuario      = dto
        });
    }

    public async Task<Result<LoginDto>> Refresh(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<LoginDto>.Unauthorized("Refresh token requerido");

        var (usuario, carreraNombre) = await Repository.GetByRefreshTokenWithCarrera(refreshToken);

        if (usuario == null || usuario.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<LoginDto>.Unauthorized("Refresh token inválido o expirado");

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = carreraNombre;

        var newAccessToken  = _jwtService.GenerateAccessToken(dto);
        var newRefreshToken = JwtService.GenerateRefreshToken();

        await Repository.UpdateRefreshToken(
            usuario.Carnet!,
            newRefreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        return Result<LoginDto>.Success(new LoginDto
        {
            AccessToken  = newAccessToken,
            RefreshToken = newRefreshToken,
            Usuario      = dto
        });
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        var deleteResult = await Repository.Delete(carnet);

        if (deleteResult.IsSuccess)
        {
            _ = await _cacheRepository.Remove(CacheKeys.Usuario(carnet));
            await Audit!.Log(AuditAccion.Eliminar, typeof(UsuarioEntity).Name, carnet);
        }

        return deleteResult;
    }

    private async Task ResolveCarrera(UsuarioDto dto)
    {
        if ((dto.IdCarrera ?? 0) > 0)
            return;

        if (string.IsNullOrWhiteSpace(dto.CarreraNombre))
            return;

        dto.IdCarrera = await Repository.FindCarreraIdByName(dto.CarreraNombre);
    }
}
