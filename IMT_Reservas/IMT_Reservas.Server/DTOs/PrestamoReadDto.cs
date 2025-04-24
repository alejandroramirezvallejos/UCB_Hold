public class PrestamoReadDto
{
    public int CodPrstm { get; set; }
    public DateTime FechaSol { get; set; }
    public TimeSpan HraSol { get; set; }
    public DateTime FechaPrstm { get; set; }
    public TimeSpan HraPrstm { get; set; }
    public DateTime FechaDev { get; set; }
    public TimeSpan HraDev { get; set; }
    public string EstadoPrstm { get; set; }
    public string EstadoDev { get; set; }
}
