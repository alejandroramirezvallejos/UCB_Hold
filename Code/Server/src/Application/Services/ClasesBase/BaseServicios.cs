using System.Data;

public class BaseServicios
{
    protected virtual void ValidarEntradaCreacion<T>(T comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    protected virtual void ValidarEntradaEliminacion<T>(T comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    protected virtual void InterpretarErrorCreacion<T>(T comando, Exception ex )
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    protected virtual void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }    
    protected virtual BaseDto MapearFilaADto(DataRow fila)
    {
        throw new NotImplementedException("Las clases hijas deben implementar MapearFilaADto para su DTO específico");
    }
}