using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Infrastructure.Config;

public sealed class SearchReindexer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SearchReindexer(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<GrupoEquipoRepository>();
        var index = scope.ServiceProvider.GetRequiredService<ISearchIndex<GrupoEquipoDto>>();
        var groups = await repository.Search();

        await index.IndexMany(groups, group => group.Id ?? 0);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
