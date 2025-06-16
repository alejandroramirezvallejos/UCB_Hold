using System.Data;
public interface IGaveteroRepository
{
    void Crear(CrearGaveteroComando comando);
    void Actualizar(ActualizarGaveteroComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
}
