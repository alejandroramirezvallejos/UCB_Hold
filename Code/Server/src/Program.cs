using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using AutoMapper;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using IMT_Reservas.Server.Presentation.Middleware;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbConfig>(
    builder.Configuration.GetSection("MongoDbSettings"));

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MongoDbContext>();

var serviceProvider = builder.Services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program)), loggerFactory);
var mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<PrestamoRepository>();
builder.Services.AddScoped<EquipoRepository>();
builder.Services.AddScoped<AccesorioRepository>();
builder.Services.AddScoped<GrupoEquipoRepository>();
builder.Services.AddScoped<CarreraRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MantenimientoRepository>();
builder.Services.AddScoped<EmpresaMantenimientoRepository>();
builder.Services.AddScoped<GaveteroRepository>();
builder.Services.AddScoped<MuebleRepository>();
builder.Services.AddScoped<ContratoRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<PrestamoService>();
builder.Services.AddScoped<EquipoService>();
builder.Services.AddScoped(sp => new Service<AccesorioEntity, AccesorioRepository, AccesorioDto>(
    sp.GetRequiredService<AccesorioRepository>()));
builder.Services.AddScoped<GrupoEquipoService>();
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MantenimientoService>();
builder.Services.AddScoped(sp => new Service<EmpresaMantenimientoEntity, EmpresaMantenimientoRepository, EmpresaMantenimientoDto>(
    sp.GetRequiredService<EmpresaMantenimientoRepository>()));
builder.Services.AddScoped<GaveteroService>();
builder.Services.AddScoped<MuebleService>();
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<ComponenteRepository>();
builder.Services.AddScoped(sp => new Service<ComponenteEntity, ComponenteRepository, ComponenteDto>(
    sp.GetRequiredService<ComponenteRepository>()));

var mongoDbConfig = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbConfig>();
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoDbConfig?.ConnectionString ?? "mongodb://localhost:27018"));

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
