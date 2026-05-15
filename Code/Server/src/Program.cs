using Microsoft.EntityFrameworkCore;
using Npgsql;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Presentation.Middleware;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
NpgsqlConnection.GlobalTypeMapper.MapEnum<EstadoPrestamo>("estado_prestamo");
NpgsqlConnection.GlobalTypeMapper.MapEnum<TipoUsuario>("tipo_usuario");
NpgsqlConnection.GlobalTypeMapper.MapEnum<EstadoEquipo>("estado_equipo");

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", corsBuilder =>
    {
        corsBuilder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<AccesorioService>();
builder.Services.AddScoped<GrupoEquipoService>();
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MantenimientoService>();

builder.Services.AddScoped<EmpresaMantenimientoService>();

builder.Services.AddScoped<GaveteroService>();
builder.Services.AddScoped<MuebleService>();
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<ComponenteRepository>();
builder.Services.AddScoped<ComponenteService>();

builder.Services.AddSingleton<UsuarioMapper>();
builder.Services.AddSingleton<PrestamoMapper>();
builder.Services.AddSingleton<EquipoMapper>();
builder.Services.AddSingleton<CarreraMapper>();
builder.Services.AddSingleton<CategoriaMapper>();
builder.Services.AddSingleton<GrupoEquipoMapper>();
builder.Services.AddSingleton<GaveteroMapper>();
builder.Services.AddSingleton<MuebleMapper>();
builder.Services.AddSingleton<MantenimientoMapper>();
builder.Services.AddSingleton<EmpresaMantenimientoMapper>();
builder.Services.AddSingleton<AccesorioMapper>();
builder.Services.AddSingleton<ComponenteMapper>();
builder.Services.AddSingleton<ContratoMapper>();

builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();

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
