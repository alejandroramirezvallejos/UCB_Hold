using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace IMT_Reservas.Server.Core.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ExecuteQuery>();
        services.AddScoped<IExecuteQuery>(sp => sp.GetRequiredService<ExecuteQuery>());
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<AccesorioRepository>();
        services.AddScoped<AccesorioService>();

        services.AddScoped<CarreraRepository>();
        services.AddScoped<CarreraService>();

        services.AddScoped<CategoriaRepository>();
        services.AddScoped<CategoriaService>();

        services.AddScoped<ComponenteRepository>();
        services.AddScoped<ComponenteService>();

        services.AddScoped<EmpresaMantenimientoRepository>();
        services.AddScoped<EmpresaMantenimientoService>();

        services.AddScoped<EquipoRepository>();
        services.AddScoped<EquipoService>();

        services.AddScoped<GaveteroRepository>();
        services.AddScoped<GaveteroService>();

        services.AddScoped<GrupoEquipoRepository>();
        services.AddScoped<GrupoEquipoService>();

        services.AddScoped<MantenimientoRepository>();
        services.AddScoped<MantenimientoService>();
        services.AddScoped<MantenimientoEquipoService>();

        services.AddScoped<MuebleRepository>();
        services.AddScoped<MuebleService>();

        services.AddScoped<PrestamoRepository>();
        services.AddScoped<PrestamoService>();

        services.AddScoped<UsuarioRepository>();
        services.AddScoped<UsuarioService>();

        services.AddScoped<CarritoRepository>();
        services.AddScoped<CarritoService>();

        return services;
    }
}
