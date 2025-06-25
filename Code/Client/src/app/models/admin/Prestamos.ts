export class Prestamos{
    Id: number = 0;
    CarnetUsuario: string | null = null;
    NombreUsuario: string | null = null;
    ApellidoPaternoUsuario: string | null = null;
    TelefonoUsuario: string | null = null;
    NombreGrupoEquipo: string | null = null;
    CodigoImt: string | null = null;
    FechaSolicitud: Date | null = null;
    FechaPrestamoEsperada: Date | null = null;
    FechaPrestamo: Date | null = null;
    FechaDevolucionEsperada: Date | null = null;
    FechaDevolucion: Date | null = null;
    Observacion: string | null = null;
    EstadoPrestamo: string | null = null;

    
    constructor(){
        this.Id = 0;
        this.CarnetUsuario = null;
        this.NombreUsuario = null;
        this.ApellidoPaternoUsuario = null;
        this.TelefonoUsuario = null;
        this.NombreGrupoEquipo = null;
        this.CodigoImt = null;
        this.FechaSolicitud = null;
        this.FechaPrestamoEsperada = null;
        this.FechaPrestamo = null;
        this.FechaDevolucionEsperada = null;
        this.FechaDevolucion = null;
        this.Observacion = null;
        this.EstadoPrestamo = null;
    }

}