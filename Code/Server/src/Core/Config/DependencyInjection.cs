using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using IMT_Reservas.Server.Infrastructure.Repositories;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
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
using IMT_Reservas.Server.Application.Features.Carrito;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace IMT_Reservas.Server.Core.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ExecuteQuery>();
        services.AddScoped<IExecuteQuery>(sp => sp.GetRequiredService<ExecuteQuery>());

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IRepository<AccesorioListDto>, AccesorioRepository>();
        services.AddScoped<AccesorioService>();

        services.AddScoped<IRepository<CarreraListDto>, CarreraRepository>();
        services.AddScoped<CarreraService>();

        services.AddScoped<IRepository<CategoriaListDto>, CategoriaRepository>();
        services.AddScoped<CategoriaService>();

        services.AddScoped<IComentarioRepository, ComentarioRepository>();
        services.AddScoped<ComentarioService>();

        services.AddScoped<IRepository<ComponenteListDto>, ComponenteRepository>();
        services.AddScoped<ComponenteService>();

        services.AddScoped<IRepository<EmpresaMantenimientoListDto>, EmpresaMantenimientoRepository>();
        services.AddScoped<EmpresaMantenimientoService>();

        services.AddScoped<IRepository<EquipoListDto>, EquipoRepository>();
        services.AddScoped<EquipoService>();

        services.AddScoped<IRepository<GaveteroListDto>, GaveteroRepository>();
        services.AddScoped<GaveteroService>();

        services.AddScoped<IRepository<GrupoEquipoListDto>, GrupoEquipoRepository>();
        services.AddScoped<GrupoEquipoService>();

        services.AddScoped<IRepository<MantenimientoListDto>, MantenimientoRepository>();
        services.AddScoped<MantenimientoService>();

        services.AddScoped<IRepository<MuebleListDto>, MuebleRepository>();
        services.AddScoped<MuebleService>();

        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        services.AddScoped<NotificacionService>();

        services.AddScoped<IRepository<PrestamoListDto>, PrestamoRepository>();
        services.AddScoped<PrestamoService>();

        services.AddScoped<IRepository<UsuarioListDto>, UsuarioRepository>();
        services.AddScoped<UsuarioService>();

        services.AddScoped<CarritoService>();
        services.AddScoped<ICarritoRepository, CarritoRepository>();

        return services;
    }
}
