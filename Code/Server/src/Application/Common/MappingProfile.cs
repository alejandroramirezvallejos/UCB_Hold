using AutoMapper;
using IMT_Reservas.Server.Application.Dtos;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;

namespace IMT_Reservas.Server.Application.Common;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UsuarioDto, UsuarioEntity>().ReverseMap();
        CreateMap<UsuarioEntity, UsuarioDetailDto>().ReverseMap();
        CreateMap<UsuarioEntity, UsuarioListDto>().ReverseMap();

        CreateMap<EquipoDto, EquipoEntity>().ReverseMap();
        CreateMap<EquipoEntity, EquipoDetailDto>().ReverseMap();
        CreateMap<EquipoEntity, EquipoListDto>().ReverseMap();

        CreateMap<AccesorioDto, AccesorioEntity>().ReverseMap();
        CreateMap<AccesorioEntity, AccesorioDetailDto>().ReverseMap();
        CreateMap<AccesorioEntity, AccesorioListDto>().ReverseMap();

        CreateMap<GrupoEquipoDto, GrupoEquipoEntity>().ReverseMap();
        CreateMap<GrupoEquipoEntity, GrupoEquipoDetailDto>().ReverseMap();
        CreateMap<GrupoEquipoEntity, GrupoEquipoListDto>().ReverseMap();

        CreateMap<CarreraDto, CarreraEntity>().ReverseMap();
        CreateMap<CarreraEntity, CarreraDetailDto>().ReverseMap();
        CreateMap<CarreraEntity, CarreraListDto>().ReverseMap();

        CreateMap<CategoriaDto, CategoriaEntity>().ReverseMap();
        CreateMap<CategoriaEntity, CategoriaDetailDto>().ReverseMap();
        CreateMap<CategoriaEntity, CategoriaListDto>().ReverseMap();

        CreateMap<PrestamoDto, PrestamoEntity>().ReverseMap();
        CreateMap<PrestamoEntity, PrestamoDetailDto>().ReverseMap();
        CreateMap<PrestamoEntity, PrestamoListDto>().ReverseMap();

        CreateMap<MantenimientoDto, MantenimientoEntity>().ReverseMap();
        CreateMap<MantenimientoEntity, MantenimientoDetailDto>().ReverseMap();
        CreateMap<MantenimientoEntity, MantenimientoListDto>().ReverseMap();

        CreateMap<EmpresaMantenimientoDto, EmpresaMantenimientoEntity>().ReverseMap();
        CreateMap<EmpresaMantenimientoEntity, EmpresaMantenimientoDetailDto>().ReverseMap();
        CreateMap<EmpresaMantenimientoEntity, EmpresaMantenimientoListDto>().ReverseMap();

        CreateMap<GaveteroDto, GaveteroEntity>().ReverseMap();
        CreateMap<GaveteroEntity, GaveteroDetailDto>().ReverseMap();
        CreateMap<GaveteroEntity, GaveteroListDto>().ReverseMap();

        CreateMap<MuebleDto, MuebleEntity>().ReverseMap();
        CreateMap<MuebleEntity, MuebleDetailDto>().ReverseMap();
        CreateMap<MuebleEntity, MuebleListDto>().ReverseMap();
    }
}
