import { Basemodel } from "../base/Basemodel";

export class Equipos extends Basemodel{
    NombreGrupoEquipo: string | null = null;
    Modelo: string | null = null;
    Marca: string | null = null;
    CodigoImt: number | null = null;
    CodigoUcb: string | null = null;
    NumeroSerial: string | null = null;
    EstadoEquipo: string | null = null;
    Ubicacion: string | null = null;
    NombreGavetero: string | null = null;
    CostoReferencia: number | null = null;
    Descripcion: string | null = null;
    TiempoMaximoPrestamo: number | null = null;
    Procedencia: string | null = null;

    constructor(){
        super();
        this.NombreGrupoEquipo = null;
        this.Modelo = null;
        this.Marca = null;
        this.CodigoImt = null;
        this.CodigoUcb = null;
        this.NumeroSerial = null;
        this.EstadoEquipo = null;
        this.Ubicacion = null;
        this.NombreGavetero = null;
        this.CostoReferencia = null;
        this.Descripcion = null;
        this.TiempoMaximoPrestamo = null;
        this.Procedencia = null;
    }


}