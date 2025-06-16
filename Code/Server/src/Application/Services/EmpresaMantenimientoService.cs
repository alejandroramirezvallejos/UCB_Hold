using System.Data;
public class EmpresaMantenimientoService
{
    private readonly EmpresaMantenimientoRepository _empresaMantenimientoRepository;
    public EmpresaMantenimientoService(EmpresaMantenimientoRepository empresaMantenimientoRepository)
    {
        _empresaMantenimientoRepository = empresaMantenimientoRepository;
    }
    public void CrearEmpresaMantenimiento(CrearEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<EmpresaMantenimientoDto>? ObtenerTodasEmpresasMantenimiento()
    {
        try
        {
            DataTable resultado = _empresaMantenimientoRepository.ObtenerTodos();
            var lista = new List<EmpresaMantenimientoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarEmpresaMantenimiento(ActualizarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarEmpresaMantenimiento(EliminarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    private static EmpresaMantenimientoDto MapearFilaADto(DataRow fila)
    {
        return new EmpresaMantenimientoDto
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
}