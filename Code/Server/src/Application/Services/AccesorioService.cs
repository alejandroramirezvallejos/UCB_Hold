using System.Data;
public class AccesorioService
{
    private readonly AccesorioRepository _accesorioRepository;
    public AccesorioService(AccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }
    public void CrearAccesorio(CrearAccesorioComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos del accesorio son requeridos");

            if (string.IsNullOrWhiteSpace(comando.Nombre))
                throw new ArgumentException("El nombre del accesorio es requerido", nameof(comando.Nombre));

            if (string.IsNullOrWhiteSpace(comando.Modelo))
                throw new ArgumentException("El modelo del accesorio es requerido", nameof(comando.Modelo));
            _accesorioRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<AccesorioDto>? ObtenerTodosAccesorios()
    {
        try
        {
            DataTable dt = _accesorioRepository.ObtenerTodos();
            var lista = new List<AccesorioDto>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                lista.Add(MapearFilaADto(row));
            return lista;
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarAccesorio(ActualizarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarAccesorio(EliminarAccesorioComando comando)
    {
        try
        {
            _accesorioRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    private static AccesorioDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id = Convert.ToInt32(fila["id_accesorio"]),
            Nombre = fila["nombre_accesorio"]==DBNull.Value? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
        };
    }
}