using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using BCryptLib = BCrypt.Net.BCrypt;
namespace IMT_Reservas.Server.Application.Features.Warmup;

public class WarmupService
{
    private readonly UsuarioRepository  _usuarioRepo;
    private readonly GrupoEquipoService _grupoSvc;

    public WarmupService(UsuarioRepository usuarioRepo, GrupoEquipoService grupoSvc)
    {
        _usuarioRepo = usuarioRepo;
        _grupoSvc    = grupoSvc;
    }

    public async Task Run()
    {
        _ = await _usuarioRepo.GetByEmailWithCarrera(string.Empty);
        _ = await _usuarioRepo.GetByRefreshTokenWithCarrera(string.Empty);
        _ = await _grupoSvc.Search(string.Empty, null);
        var hash = BCryptLib.HashPassword("x", workFactor: 4);
        
        BCryptLib.Verify("x", hash);
    }
}
