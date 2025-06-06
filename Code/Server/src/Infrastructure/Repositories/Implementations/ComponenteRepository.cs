//implementar
using System.Data;
public class ComponenteRepository : IComponenteRepository
{
        private readonly IExecuteQuery _ejecutarConsulta;

        public ComponenteRepository(IExecuteQuery ejecutarConsulta)
        {
            _ejecutarConsulta = ejecutarConsulta;
        }

        public ComponenteDto Crear(CrearComponenteComando comando)
        {
            // Implementar lógica para crear un nuevo componente
        }

        public ComponenteDto? ObtenerPorId(int id)
        {
            // Implementar lógica para obtener un componente por su ID
        }

        public ComponenteDto? Actualizar(ActualizarComponenteComando comando)
        {
            // Implementar lógica para actualizar un componente existente
        }

        public bool Eliminar(int id)
        {
            // Implementar lógica para eliminar un componente por su ID
        }

        public List<ComponenteDto> ObtenerTodos()
        {
            // Implementar lógica para obtener todos los componentes
        }
}