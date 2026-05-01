namespace IMT_Reservas.Server.Application.Commands.EmpresaMantenimiento;

public record CrearEmpresaMantenimientoComando(
    string? NombreEmpresa,
    string? NombreResponsable,
    string? ApellidoResponsable,
    string? Telefono,
    string? Direccion,
    string? Nit
);