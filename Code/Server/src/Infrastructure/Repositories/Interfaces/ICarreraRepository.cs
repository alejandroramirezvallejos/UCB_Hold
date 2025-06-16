using System.Data;
public interface ICarreraRepository
{
    void Crear(CrearCarreraComando comando);
    void Actualizar(ActualizarCarreraComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodas();
}