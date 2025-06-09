namespace API.ViewModels;

public class PrestamoRequestDto
{
    public int[]?    GrupoEquipoId           { get; set; } = null;
    public DateTime? FechaPrestamoEsperada   { get; set; }= null;
    public DateTime? FechaDevolucionEsperada { get; set; }= null;
    public string?  Observacion             { get; set; } = null;
    public string?   CarnetUsuario           { get; set; } = null;
    public byte[]?  Contrato                { get; set; }= null;
}