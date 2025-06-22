using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class CarreraService : ICarreraService
{
    private readonly ICarreraRepository _carreraRepository;
    public CarreraService(ICarreraRepository carreraRepository) => _carreraRepository = carreraRepository;

    public void CrearCarrera(CrearCarreraComando comando)
    {
        ValidarEntradaCreacion(comando);
        _carreraRepository.Crear(comando);
    }

    public List<CarreraDto>? ObtenerTodasCarreras()
    {
        DataTable resultado = _carreraRepository.ObtenerTodas();
        var lista = new List<CarreraDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    public void ActualizarCarrera(ActualizarCarreraComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _carreraRepository.Actualizar(comando);
    }

    public void EliminarCarrera(EliminarCarreraComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _carreraRepository.Eliminar(comando.Id);
    }

    private void ValidarEntradaCreacion(CrearCarreraComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre de la carrera", 256);
    }
    private void ValidarEntradaActualizacion(ActualizarCarreraComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre de la carrera", 255);
    }
    private void ValidarEntradaEliminacion(EliminarCarreraComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private CarreraDto MapearFilaADto(DataRow fila) => new CarreraDto
    {
        Id = Convert.ToInt32(fila["id_carrera"]),
        Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
    };
}