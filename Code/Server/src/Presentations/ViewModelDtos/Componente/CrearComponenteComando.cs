//TODO: Implementar
public record CrearComponenteComando(
    string?  Nombre,    //Se valida si no es nulo     
    string?  Modelo,    //Se valida si no es nulo      
    string? Tipo,             
    int?     CodigoIMT,        //Se valida si no es nulo
    string? Descripcion,     
    double? PrecioReferencia, 
    string? UrlDataSheet              
);