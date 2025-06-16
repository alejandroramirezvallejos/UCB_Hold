using System.Data;
public class MuebleService : IMuebleService
{
    private readonly MuebleRepository _muebleRepository;

    public MuebleService(MuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }    public void CrearMueble(CrearMuebleComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos del mueble son requeridos");

            if (string.IsNullOrWhiteSpace(comando.Nombre))
                throw new ArgumentException("El nombre del mueble es requerido", nameof(comando.Nombre));

            if (comando.Costo.HasValue && comando.Costo < 0)
                throw new ArgumentException("El costo no puede ser negativo", nameof(comando.Costo));

            if (comando.Longitud.HasValue && comando.Longitud <= 0)
                throw new ArgumentException("La longitud debe ser mayor a 0", nameof(comando.Longitud));

            if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
                throw new ArgumentException("La profundidad debe ser mayor a 0", nameof(comando.Profundidad));

            if (comando.Altura.HasValue && comando.Altura <= 0)
                throw new ArgumentException("La altura debe ser mayor a 0", nameof(comando.Altura));

            _muebleRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarMueble(ActualizarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarMueble(EliminarMuebleComando comando)
    {
        try
        {
            _muebleRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    public List<MuebleDto>? ObtenerTodosMuebles()
    {
        try
        {
            DataTable resultado = _muebleRepository.ObtenerTodos();
            var lista = new List<MuebleDto>(resultado.Rows.Count);
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
    private MuebleDto MapearFilaADto(DataRow fila)
    {
        return new MuebleDto
        {
            Id = Convert.ToInt32(fila["id_mueble"]),
            Nombre = fila["nombre_mueble"] == DBNull.Value ? null : fila["nombre_mueble"].ToString(),
            NumeroGaveteros = fila["numero_gaveteros_mueble"] == DBNull.Value ? null : Convert.ToInt32(fila["numero_gaveteros_mueble"]),
            Ubicacion = fila["ubicacion_mueble"] == DBNull.Value ? null : fila["ubicacion_mueble"].ToString(),
            Tipo = fila["tipo_mueble"] == DBNull.Value ? null : fila["tipo_mueble"].ToString(),
            Costo = fila["costo_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mueble"]),
            Longitud = fila["longitud_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_mueble"]),
            Profundidad = fila["profundidad_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_mueble"]),
            Altura = fila["altura_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_mueble"])
        };
    }
}