public interface IActualizarPrestamoComando
{
    PrestamoDto? Handle(ActualizarPrestamoComando comando);
}