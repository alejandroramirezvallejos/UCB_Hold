import { Basemodel } from "../base/Basemodel";

export class Mantenimientos extends Basemodel{

    NombreEmpresaMantenimiento: string | null = null;
    FechaMantenimiento: Date | null = null;
    FechaFinalDeMantenimiento: Date | null = null;
    Costo: number | null = null;
    Descripcion: string | null = null;
    TipoMantenimiento: string | null = null;
    NombreGrupoEquipo: string | null = null;
    CodigoImtEquipo: number | null = null;
    DescripcionEquipo: string | null = null;

    constructor(){
        super();
        this.NombreEmpresaMantenimiento = null;
        this.FechaMantenimiento = null;
        this.FechaFinalDeMantenimiento = null;
        this.Costo = null;
        this.Descripcion = null;
        this.TipoMantenimiento = null;
        this.NombreGrupoEquipo = null;
        this.CodigoImtEquipo = null;
        this.DescripcionEquipo = null;
    }


}