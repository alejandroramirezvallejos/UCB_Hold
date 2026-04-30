using System.Data;

public abstract class Service
{
    protected virtual Dto MapearFilaADto(DataRow fila) =>
        throw new NotImplementedException("Las clases hijas deben implementar MapearFilaADto para su DTO específico");
}
