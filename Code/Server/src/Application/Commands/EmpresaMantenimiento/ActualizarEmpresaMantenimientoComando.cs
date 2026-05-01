namespace IMT_Reservas.Server.Application.Commands.EmpresaMantenimiento;

public record ActualizarEmpresaMantenimientoComando(
    int Id,
    string? NombreEmpresa,
    string? NombreResponsable,
    string? ApellidoResponsable,
    string? Telefono,
    string? Direccion,
    string? Nit
);