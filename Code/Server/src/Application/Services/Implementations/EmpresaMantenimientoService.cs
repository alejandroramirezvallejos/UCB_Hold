using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class EmpresaMantenimientoService : IEmpresaMantenimientoService
{
    private readonly IEmpresaMantenimientoRepository _empresaMantenimientoRepository;
    public EmpresaMantenimientoService(IEmpresaMantenimientoRepository empresaMantenimientoRepository)
    {
        _empresaMantenimientoRepository = empresaMantenimientoRepository;
    }
    public virtual void CrearEmpresaMantenimiento(CrearEmpresaMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _empresaMantenimientoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe una empresa de mantenimiento con esos datos"))
                    throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error al insertar empresa de mantenimiento"))
                    throw new Exception($"Error inesperado al insertar empresa de mantenimiento: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al crear empresa de mantenimiento: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo)
                throw new Exception($"Error del repositorio al crear empresa de mantenimiento: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaCreacion(CrearEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.NombreEmpresa)) throw new ErrorNombreRequerido();
        if (comando.NombreEmpresa.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Telefono) && comando.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
    }
    public virtual List<EmpresaMantenimientoDto>? ObtenerTodasEmpresasMantenimiento()
    {
        try
        {
            DataTable resultado = _empresaMantenimientoRepository.ObtenerTodos();
            var lista = new List<EmpresaMantenimientoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
                lista.Add(MapearFilaADto(fila));
            return lista;
        }
        catch { throw; }
    }
    public virtual void ActualizarEmpresaMantenimiento(ActualizarEmpresaMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _empresaMantenimientoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró una empresa de mantenimiento activa con id"))
                    throw new ErrorRegistroNoEncontrado();
                if (errorDb.SqlState == "23505" || mensaje.Contains("error de violación de unicidad"))
                    throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar la empresa de mantenimiento"))
                    throw new Exception($"Error inesperado al actualizar empresa de mantenimiento: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar empresa de mantenimiento: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo)
                throw new Exception($"Error del repositorio al actualizar empresa de mantenimiento: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    public virtual void EliminarEmpresaMantenimiento(EliminarEmpresaMantenimientoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _empresaMantenimientoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un registro activo en empresas_mantenimiento con id"))
                    throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("error al eliminar lógicamente empresas_mantenimiento"))
                    throw new Exception($"Error inesperado al eliminar empresa de mantenimiento: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al eliminar empresa de mantenimiento: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo)
                throw new Exception($"Error del repositorio al eliminar empresa de mantenimiento: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaActualizacion(ActualizarEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.NombreEmpresa) && comando.NombreEmpresa.Length > 255)
            throw new ErrorLongitudInvalida("nombre de la empresa", 255);
    }
    private void ValidarEntradaEliminacion(EliminarEmpresaMantenimientoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido();
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