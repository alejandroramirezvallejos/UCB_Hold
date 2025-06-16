using System.Data;
public class ComponenteService
{
    private readonly ComponenteRepository _componenteRepository;
    public ComponenteService(ComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }
    public void CrearComponente(CrearComponenteComando comando)
    {
        try
        {
            _componenteRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<ComponenteDto>? ObtenerTodosComponentes()
    {
        try
        {
            DataTable resultado = _componenteRepository.ObtenerTodos();
            var lista = new List<ComponenteDto>(resultado.Rows.Count);
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
    public void ActualizarComponente(ActualizarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarComponente(EliminarComponenteComando comando)
    {
        try
        {
            _componenteRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    private static ComponenteDto MapearFilaADto(DataRow fila)
    {
        return new ComponenteDto
        {
            Id = Convert.ToInt32(fila["id_componente"]),
            Nombre = fila["nombre_componente"] == DBNull.Value ? null : fila["nombre_componente"].ToString(),
            Modelo = fila["modelo_componente"] == DBNull.Value ? null : fila["modelo_componente"].ToString(),
            Tipo = fila["tipo_componente"] == DBNull.Value ? null : fila["tipo_componente"].ToString(),
            Descripcion = fila["descripcion_componente"] == DBNull.Value ? null : fila["descripcion_componente"].ToString(),
            PrecioReferencia = fila["precio_referencia_componente"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_referencia_componente"]),
            NombreEquipo = fila["nombre_equipo"] == DBNull.Value ? null : fila["nombre_equipo"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"])
        };
    }
}