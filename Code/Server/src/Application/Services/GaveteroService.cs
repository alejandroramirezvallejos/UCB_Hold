using System.Data;
public class GaveteroService : IGaveteroService
{
    private readonly GaveteroRepository _gaveteroRepository;

    public GaveteroService(GaveteroRepository gaveteroRepository)
    {
        _gaveteroRepository = gaveteroRepository;
    }
    public void CrearGavetero(CrearGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarGavetero(ActualizarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarGavetero(EliminarGaveteroComando comando)
    {
        try
        {
            _gaveteroRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    public List<GaveteroDto>? ObtenerTodosGaveteros()
    {
        try
        {
            DataTable resultado = _gaveteroRepository.ObtenerTodos();
            var lista = new List<GaveteroDto>(resultado.Rows.Count);
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
    private GaveteroDto MapearFilaADto(DataRow fila)
    {
        return new GaveteroDto
        {
            Id = Convert.ToInt32(fila["id_gavetero"]),
            Nombre = fila["nombre_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["nombre_gavetero"]),
            Tipo = fila["tipo_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["tipo_gavetero"]),
            NombreMueble = fila["nombre_mueble"] == DBNull.Value ? null : Convert.ToString(fila["nombre_mueble"]),
            Longitud = fila["longitud_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_gavetero"]),
            Profundidad = fila["profundidad_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_gavetero"]),
            Altura = fila["altura_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_gavetero"])
        };
    }
}