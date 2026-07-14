using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace IMT_Reservas.Tests.Helpers;

internal abstract class ServiceTest<TService>
{
    private string _dbName = string.Empty;

    protected ApplicationDbContext Db { get; private set; } = null!;
    protected TService Sut { get; private set; } = default!;

    protected abstract TService CreateService(ApplicationDbContext db);

    [SetUp]
    public void SetUp()
    {
        _dbName = Guid.NewGuid().ToString();
        Db = NewDbContext();
        Sut = CreateService(Db);
    }

    protected ApplicationDbContext NewDbContext()
        => new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options);

    protected TService NewSut() => CreateService(NewDbContext());

    [TearDown]
    public void TearDown() => Db.Dispose();
}
