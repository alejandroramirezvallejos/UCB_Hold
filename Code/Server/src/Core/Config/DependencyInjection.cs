using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Application.Features.Carrito;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Application.Features.Comentario;
using IMT_Reservas.Server.Application.Features.Comentario.Dtos;
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
using IMT_Reservas.Server.Application.Features.Notificacion;
using IMT_Reservas.Server.Application.Features.Notificacion.Dtos;
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
        services.AddScoped<IRepository<AccesorioListDto>>(sp => sp.GetRequiredService<AccesorioRepository>());
        services.AddScoped<AccesorioService>();

        services.AddScoped<CarreraRepository>();
        services.AddScoped<IRepository<CarreraListDto>>(sp => sp.GetRequiredService<CarreraRepository>());
        services.AddScoped<CarreraService>();

        services.AddScoped<CategoriaRepository>();
        services.AddScoped<IRepository<CategoriaListDto>>(sp => sp.GetRequiredService<CategoriaRepository>());
        services.AddScoped<CategoriaService>();

        services.AddScoped<ComentarioRepository>();
        services.AddScoped<ComentarioRepository>(sp => sp.GetRequiredService<ComentarioRepository>());
        services.AddScoped<ComentarioService>();

        services.AddScoped<ComponenteRepository>();
        services.AddScoped<IRepository<ComponenteListDto>>(sp => sp.GetRequiredService<ComponenteRepository>());
        services.AddScoped<ComponenteService>();

        services.AddScoped<EmpresaMantenimientoRepository>();
        services.AddScoped<IRepository<EmpresaMantenimientoListDto>>(sp => sp.GetRequiredService<EmpresaMantenimientoRepository>());
        services.AddScoped<EmpresaMantenimientoService>();

        services.AddScoped<EquipoRepository>();
        services.AddScoped<IRepository<EquipoListDto>>(sp => sp.GetRequiredService<EquipoRepository>());
        services.AddScoped<EquipoService>();

        services.AddScoped<GaveteroRepository>();
        services.AddScoped<IRepository<GaveteroListDto>>(sp => sp.GetRequiredService<GaveteroRepository>());
        services.AddScoped<GaveteroService>();

        services.AddScoped<GrupoEquipoRepository>();
        services.AddScoped<IRepository<GrupoEquipoListDto>>(sp => sp.GetRequiredService<GrupoEquipoRepository>());
        services.AddScoped<GrupoEquipoService>();

        services.AddScoped<MantenimientoRepository>();
        services.AddScoped<IRepository<MantenimientoListDto>>(sp => sp.GetRequiredService<MantenimientoRepository>());
        services.AddScoped<MantenimientoService>();

        services.AddScoped<MuebleRepository>();
        services.AddScoped<IRepository<MuebleListDto>>(sp => sp.GetRequiredService<MuebleRepository>());
        services.AddScoped<MuebleService>();

        services.AddScoped<NotificacionRepository>();
        services.AddScoped<NotificacionRepository>(sp => sp.GetRequiredService<NotificacionRepository>());
        services.AddScoped<NotificacionService>();

        services.AddScoped<PrestamoRepository>();
        services.AddScoped<IRepository<PrestamoListDto>>(sp => sp.GetRequiredService<PrestamoRepository>());
        services.AddScoped<PrestamoService>();

        services.AddScoped<UsuarioRepository>();
        services.AddScoped<IRepository<UsuarioListDto>>(sp => sp.GetRequiredService<UsuarioRepository>());
        services.AddScoped<UsuarioService>();

        services.AddScoped<CarritoRepository>();
        services.AddScoped<CarritoRepository>(sp => sp.GetRequiredService<CarritoRepository>());
        services.AddScoped<CarritoService>();

        return services;
    }
}
