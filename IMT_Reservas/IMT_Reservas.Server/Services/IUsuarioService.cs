public interface IUsuarioService
{
    //INFO: Obtiene un usuario por su carnet; devuelve null si no existe
    UsuarioReadDto? ObtenerUsuarioPorCarnet(string carnet);

    //INFO: Crea un nuevo usuario a partir de la entidad de dominio
    void CrearUsuario(Usuario usuario);
}
