using System.Data;

public class CarreraService : ICarreraService
{
    private readonly CarreraRepository _carreraRepository;
    public CarreraService(CarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }    public void CrearCarrera(CrearCarreraComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos de la carrera son requeridos");

            if (string.IsNullOrWhiteSpace(comando.Nombre))
                throw new ArgumentException("El nombre de la carrera es requerido", nameof(comando.Nombre));

            if (comando.Nombre.Length > 100)
                throw new ArgumentException("El nombre de la carrera no puede exceder 100 caracteres", nameof(comando.Nombre));

            _carreraRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<CarreraDto>? ObtenerTodasCarreras()
    {
        try
        {
            DataTable resultado = _carreraRepository.ObtenerTodas();
            var lista = new List<CarreraDto>(resultado.Rows.Count);
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
    public void ActualizarCarrera(ActualizarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarCarrera(EliminarCarreraComando comando)
    {
        try
        {
            _carreraRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    private CarreraDto MapearFilaADto(DataRow fila)
    {
        return new CarreraDto
        {
            Id = Convert.ToInt32(fila["id_carrera"]),
            Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
        };
    }
}