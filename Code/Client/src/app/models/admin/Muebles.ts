import { Basemodel } from "../base/Basemodel";

export class Muebles extends Basemodel{

    Nombre: string | null = null;
    NumeroGaveteros: number | null = null;
    Ubicacion: string | null = null;
    Tipo: string | null = null;
    Costo: number | null = null;
    Longitud: number | null = null;
    Profundidad: number | null = null;
    Altura: number | null = null;

    constructor(){
        super();
        this.Nombre = null;
        this.NumeroGaveteros = null;
        this.Ubicacion = null;
        this.Tipo = null;
        this.Costo = null;
        this.Longitud = null;
        this.Profundidad = null;
        this.Altura = null;
    }


}