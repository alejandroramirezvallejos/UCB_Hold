namespace IMT_Reservas.Server.Application.DTOs.Response;

﻿public class NotificacionDto : Dto
{
    public new string Id { get; set; } = null!;
    public string? CarnetUsuario { get; set; }
    public string? Titulo { get; set; }
    public string? Contenido { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public bool Leido { get; set; }
}