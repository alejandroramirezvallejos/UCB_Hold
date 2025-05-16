public interface IActualizarUsuarioComando
{
    UsuarioDto? Handle(ActualizarUsuarioComando comando);
}