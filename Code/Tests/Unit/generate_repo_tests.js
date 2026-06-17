const fs = require('fs');

const repos = [
    { name: 'Categoria', plural: 'Categorias' },
    { name: 'Componente', plural: 'Componentes' },
    { name: 'Contrato', plural: 'Contratos' },
    { name: 'EmpresaMantenimiento', plural: 'EmpresasMantenimiento' },
    { name: 'Gavetero', plural: 'Gaveteros' },
    { name: 'Mueble', plural: 'Muebles' },
    { name: 'Carrera', plural: 'Carreras' }
];

const template = (name, plural) => `using FluentAssertions;
using IMT_Reservas.Server.Application.Features.${name};
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IMT_Reservas.Tests.Unit.Repositories;

[TestFixture]
public class ${name}RepositoryTests
{
    private ApplicationDbContext _dbContext = null!;
    private ${name}Repository _repository = null!;
    private ${name}Mapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _mapper = new ${name}Mapper();
        _repository = new ${name}Repository(_dbContext, _mapper);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Get_ExistingId_ReturnsDto()
    {
        var entity = new ${name} { Id = 1, EstadoEliminado = false };
        _dbContext.${plural}.Add(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Get(1);

        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task GetAll_ReturnsList()
    {
        _dbContext.${plural}.Add(new ${name} { Id = 1, EstadoEliminado = false });
        _dbContext.${plural}.Add(new ${name} { Id = 2, EstadoEliminado = false });
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Test]
    public async Task Create_ValidEntity_ReturnsCreated()
    {
        var entity = new ${name} { Id = 1, EstadoEliminado = false };
        var result = await _repository.Create(entity);

        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Delete_ExistingId_ReturnsSuccess()
    {
        var entity = new ${name} { Id = 1, EstadoEliminado = false };
        _dbContext.${plural}.Add(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Delete(1);
        result.IsSuccess.Should().BeTrue();
    }
}
`;

for (const r of repos) {
    fs.writeFileSync(`${r.name}RepositoryTests.cs`, template(r.name, r.plural));
}
