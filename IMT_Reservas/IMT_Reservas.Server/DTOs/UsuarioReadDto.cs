public record UsuarioReadDto
{
    public string Carnet             { get; init; }
    public string Nombre             { get; init; }
    public string ApellidoPaterno    { get; init; }
    public string ApellidoMaterno    { get; init; }
    public string Rol                { get; init; }
    public int    CarreraId          { get; init; }
    public string Email              { get; init; }
    public string Telefono           { get; init; }
    public string NombreReferencia   { get; init; }
    public string TelefonoReferencia { get; init; }
    public string EmailReferencia    { get; init; }
    public bool   EstaEliminado      { get; init; }
}