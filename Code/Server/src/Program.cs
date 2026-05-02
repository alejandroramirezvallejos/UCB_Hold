using AutoMapper;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IMT_Reservas.Server.Infrastructure.MongoDb.MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExecuteQuery>();
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
