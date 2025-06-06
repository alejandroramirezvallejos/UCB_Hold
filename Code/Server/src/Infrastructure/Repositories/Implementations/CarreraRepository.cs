//implementar
using System.Data;
public class CarreraRepository : ICarreraRepository
{
        private readonly IExecuteQuery _ejecutarConsulta;

        public CarreraRepository(IExecuteQuery ejecutarConsulta)
        {
            _ejecutarConsulta = ejecutarConsulta;
        }

        public CarreraDto Crear(CrearCarreraComando comando)
        {
            // Implementar lógica para crear una nueva carrera
        }

        public CarreraDto? ObtenerPorId(int id)
        {
            // Implementar lógica para obtener una carrera por su ID
        }

        public CarreraDto? Actualizar(ActualizarCarreraComando comando)
        {
            // Implementar lógica para actualizar una carrera existente
        }

        public bool Eliminar(int id)
        {
            // Implementar lógica para eliminar una carrera por su ID
        }

        public List<CarreraDto> ObtenerTodas()
        {
            // Implementar lógica para obtener todas las carreras
        }
}