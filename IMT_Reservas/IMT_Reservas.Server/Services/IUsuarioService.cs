public interface IUsuarioService
{
    //INFO: Obtiene un usuario por su carnet; devuelve null si no existe
    Task<UsuarioReadDto?> ObtenerUsuarioPorCarnetAsync(string carnet);

    //INFO: Crea un nuevo usuario a partir de la entidad de dominio
    Task CrearUsuarioAsync(Usuario usuario);
}
