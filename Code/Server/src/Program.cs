using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Application.Features.Archivo;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using IMT_Reservas.Server.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbConfiguracion>(
    builder.Configuration.GetSection("MongoDbSettings"));

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MongoDbContexto>();
builder.Services.AddAutoMapper(typeof(Program));
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
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<PrestamoService>();
builder.Services.AddScoped<EquipoService>();
builder.Services.AddScoped<AccesorioService>();
builder.Services.AddScoped<GrupoEquipoService>();
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MantenimientoService>();
builder.Services.AddScoped<EmpresaMantenimientoService>();
builder.Services.AddScoped<GaveteroService>();
builder.Services.AddScoped<MuebleService>();
builder.Services.AddScoped<ComponenteRepository>();
builder.Services.AddScoped<ComponenteService>();
builder.Services.AddScoped<CarritoRepository>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<MantenimientoEquipoService>();
builder.Services.AddScoped<ContratoRepository>();
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<ArchivoService>();

builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27018/"));

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
