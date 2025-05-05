public interface IActualizarUsuarioComando
{
    UsuarioResponseDto? Handle(ActualizarUsuarioComando comando);
}