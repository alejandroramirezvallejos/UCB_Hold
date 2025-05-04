public interface ICrearUsuarioComando
{
    UsuarioResponseDto Handle(CrearUsuarioComando comando);
}