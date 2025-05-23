public interface IGavetero
{
    int     Id       { get; }
    string  Nombre   { get; }
    string? Tipo     { get; }
    int     MuebleId { get; }
}
