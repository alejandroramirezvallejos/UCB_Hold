public class UsuarioUseCase : ICrearUsuarioComando, IObtenerUsuarioConsulta, 
                              IActualizarUsuarioComando, IEliminarUsuarioComando
{
    public UsuarioResponseDto Handle(CrearUsuarioComando comando)
    {
        UsuarioResponseDto respuesta = new UsuarioResponseDto
        {
            Carnet             = comando.Carnet,
            Nombre             = comando.Nombre,
            ApellidoPaterno    = comando.ApellidoPaterno,
            ApellidoMaterno    = comando.ApellidoMaterno,
            Rol                = comando.Rol,
            CarreraId          = comando.CarreraId,
            Email              = comando.Email,
            Telefono           = comando.Telefono,
            NombreReferencia   = comando.NombreReferencia,
            TelefonoReferencia = comando.TelefonoReferencia,
            EmailReferencia    = comando.EmailReferencia,
            EstaEliminado      = false
        };

        return respuesta;
    }

    public UsuarioResponseDto? Handle(ObtenerUsuarioConsulta consulta)
    {
        return null;
    }

    public UsuarioResponseDto? Handle(ActualizarUsuarioComando comando)
    {
        return null;
    }

    public bool Handle(EliminarUsuarioComando comando)
    {
        return true;
    }
}