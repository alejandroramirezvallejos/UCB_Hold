using System.Data;

public class ServiciosAbstraccion
{
    public virtual void ValidarEntradaCreacion<T>(T comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    public virtual void ValidarEntradaEliminacion<T>(T comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    public virtual void InterpretarErrorCreacion<T>(T comando, Exception ex = null)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }
    public virtual void InterpretarErrorEliminacion<T>(T comando, Exception ex = null)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
    }    
    public virtual BaseDto MapearFilaADto(DataRow fila)
    {
        throw new NotImplementedException("Las clases hijas deben implementar MapearFilaADto para su DTO específico");
    }
}