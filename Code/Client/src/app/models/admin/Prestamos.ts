import { Basemodel } from "../base/Basemodel";

export class Prestamos extends Basemodel{

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

    IdContrato: number | null = null;
    FileId : number | null = null;

    
    constructor(){
        super();
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
        this.IdContrato = null;
        this.FileId = null;
    }

}