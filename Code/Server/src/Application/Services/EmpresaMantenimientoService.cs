using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class EmpresaMantenimientoService : IEmpresaMantenimientoService
{
    private readonly IEmpresaMantenimientoRepository _empresaMantenimientoRepository;
    public EmpresaMantenimientoService(IEmpresaMantenimientoRepository empresaMantenimientoRepository) => _empresaMantenimientoRepository = empresaMantenimientoRepository;

    public void CrearEmpresaMantenimiento(CrearEmpresaMantenimientoComando comando)
    {
        ValidarEntradaCreacion(comando);
        _empresaMantenimientoRepository.Crear(comando);
    }

    public List<EmpresaMantenimientoDto>? ObtenerTodasEmpresasMantenimiento()
    {
        DataTable resultado = _empresaMantenimientoRepository.ObtenerTodos();
        var lista = new List<EmpresaMantenimientoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    public void ActualizarEmpresaMantenimiento(ActualizarEmpresaMantenimientoComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _empresaMantenimientoRepository.Actualizar(comando);
    }

    public void EliminarEmpresaMantenimiento(EliminarEmpresaMantenimientoComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _empresaMantenimientoRepository.Eliminar(comando.Id);
    }

    private void ValidarEntradaCreacion(CrearEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.NombreEmpresa)) throw new ErrorNombreRequerido();
        if (comando.NombreEmpresa.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Telefono) && comando.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
    }
    private void ValidarEntradaActualizacion(ActualizarEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.NombreEmpresa) && comando.NombreEmpresa.Length > 255) throw new ErrorLongitudInvalida("nombre de la empresa", 255);
    }
    private void ValidarEntradaEliminacion(EliminarEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static EmpresaMantenimientoDto MapearFilaADto(DataRow fila) => new EmpresaMantenimientoDto
    {
        Id = Convert.ToInt32(fila["id_empresa_mantenimiento"]),
        NombreEmpresa = fila["nombre_empresa"] == DBNull.Value ? null : fila["nombre_empresa"].ToString(),
        NombreResponsable = fila["nombre_responsable_empresa"] == DBNull.Value ? null : fila["nombre_responsable_empresa"].ToString(),
        ApellidoResponsable = fila["apellido_responsable_empresa"] == DBNull.Value ? null : fila["apellido_responsable_empresa"].ToString(),
        Telefono = fila["telefono_empresa"] == DBNull.Value ? null : fila["telefono_empresa"].ToString(),
        Direccion = fila["direccion_empresa"] == DBNull.Value ? null : fila["direccion_empresa"].ToString(),
        Nit = fila["nit_empresa"] == DBNull.Value ? null : fila["nit_empresa"].ToString()
    };
}