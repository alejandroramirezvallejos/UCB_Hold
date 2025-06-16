using System.Data;
public class MantenimientoService
{
    private readonly MantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(MantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }
    public void CrearMantenimiento(CrearMantenimientoComando comando)
    {
        try
        {
            if (comando == null)
                throw new Exception("Los datos del mantenimiento son requeridos");

            if (comando.FechaFinalDeMantenimiento < comando.FechaMantenimiento)
                throw new Exception("La fecha final debe ser posterior a la fecha de inicio");

            if (string.IsNullOrWhiteSpace(comando.NombreEmpresaMantenimiento))
                throw new Exception("El nombre de la empresa de mantenimiento es requerido");

            if (comando.CodigoIMT == null || comando.CodigoIMT.Length == 0)
                throw new Exception("Al menos un código IMT es requerido");

            if (comando.TipoMantenimiento == null || comando.TipoMantenimiento.Length == 0)
                throw new Exception("Al menos un tipo de mantenimiento es requerido");

            if (comando.CodigoIMT.Length != comando.TipoMantenimiento.Length)
                throw new Exception("El número de códigos IMT debe coincidir con el número de tipos de mantenimiento");

            if (comando.DescripcionEquipo != null && comando.DescripcionEquipo.Length > 0 &&
                comando.DescripcionEquipo.Length != comando.CodigoIMT.Length)
                throw new Exception("Si se proporcionan descripciones de equipo, debe haber una por cada código IMT");

            if (comando.CodigoIMT.Any(codigo => codigo <= 0))
                throw new Exception("Todos los códigos IMT deben ser números positivos");
            if (comando.Costo.HasValue && comando.Costo.Value < 0)
                throw new Exception("El costo debe ser un número positivo");
            _mantenimientoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarMantenimiento(EliminarMantenimientoComando comando)
    {
        try
        {
            _mantenimientoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    public List<MantenimientoDto>? ObtenerTodosMantenimientos()
    {
        try
        {
            DataTable resultado = _mantenimientoRepository.ObtenerTodos();
            var lista = new List<MantenimientoDto>(resultado.Rows.Count);
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
    private MantenimientoDto MapearFilaADto(DataRow fila)
    {
        return new MantenimientoDto
        {
            Id = Convert.ToInt32(fila["id_mantenimiento"]),
            FechaMantenimiento = fila["fecha_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_mantenimiento"])),
            FechaFinalDeMantenimiento = fila["fecha_final_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_final_mantenimiento"])),
            NombreEmpresaMantenimiento = fila["nombre_empresa_mantenimiento"] == DBNull.Value ? null : fila["nombre_empresa_mantenimiento"].ToString(),
            Costo = fila["costo_mantenimiento"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mantenimiento"]),
            Descripcion = fila["descripcion_mantenimiento"] == DBNull.Value ? null : fila["descripcion_mantenimiento"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            TipoMantenimiento = fila["tipo_detalle_mantenimiento"] == DBNull.Value ? null : fila["tipo_detalle_mantenimiento"].ToString(),
            DescripcionEquipo = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString()
        };
    }
}