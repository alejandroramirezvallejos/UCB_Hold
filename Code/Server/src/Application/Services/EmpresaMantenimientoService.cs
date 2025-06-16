using System.Data;
public class EmpresaMantenimientoService : IEmpresaMantenimientoService
{
    private readonly EmpresaMantenimientoRepository _empresaMantenimientoRepository;
    public EmpresaMantenimientoService(EmpresaMantenimientoRepository empresaMantenimientoRepository)
    {
        _empresaMantenimientoRepository = empresaMantenimientoRepository;
    }    public void CrearEmpresaMantenimiento(CrearEmpresaMantenimientoComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos de la empresa de mantenimiento son requeridos");

            if (string.IsNullOrWhiteSpace(comando.NombreEmpresa))
                throw new ArgumentException("El nombre de la empresa es requerido", nameof(comando.NombreEmpresa));

            if (comando.NombreEmpresa.Length > 100)
                throw new ArgumentException("El nombre de la empresa no puede exceder 100 caracteres", nameof(comando.NombreEmpresa));

            if (!string.IsNullOrWhiteSpace(comando.Telefono) && comando.Telefono.Length > 20)
                throw new ArgumentException("El teléfono no puede exceder 20 caracteres", nameof(comando.Telefono));

            if (!string.IsNullOrWhiteSpace(comando.Nit) && comando.Nit.Length > 20)
                throw new ArgumentException("El NIT no puede exceder 20 caracteres", nameof(comando.Nit));

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
            Nit = fila["nit_empresa"] == DBNull.Value ? null : fila["nit_empresa"].ToString()        };
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}