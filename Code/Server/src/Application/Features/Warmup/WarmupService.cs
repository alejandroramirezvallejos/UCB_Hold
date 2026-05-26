using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using BCryptLib = BCrypt.Net.BCrypt;
namespace IMT_Reservas.Server.Application.Features.Warmup;

public class WarmupService
{
    private readonly UsuarioRepository  _usuarioRepository;
    private readonly GrupoEquipoService _grupoService;

    public WarmupService(UsuarioRepository usuarioRepository, GrupoEquipoService grupoService)
    {
        _usuarioRepository = usuarioRepository;
        _grupoService    = grupoService;
    }

    public async Task Run()
    {
        _ = await _usuarioRepository.GetByEmailWithCarrera(string.Empty);
        _ = await _usuarioRepository.GetByRefreshTokenWithCarrera(string.Empty);
        _ = await _grupoService.Search(string.Empty, null);
        var hash = BCryptLib.HashPassword("x", workFactor: 4);
        
        BCryptLib.Verify("x", hash);
    }
}
