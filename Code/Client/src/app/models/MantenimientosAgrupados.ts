import { Mantenimientos } from "./admin/Mantenimientos";

export class MantenimientosAgrupados {
   datosgrupo : {
        Id: number ;
        NombreEmpresaMantenimiento: string | null ;
        FechaMantenimiento: Date | null ;
        FechaFinalDeMantenimiento: Date | null ;
        Costo: number | null ;
        Descripcion: string | null ;
        TipoMantenimiento: string | null ;
        NombreGrupoEquipo: string | null ;
        CodigoImtEquipo: string | null ;
        DescripcionEquipo: string | null ;
   } ; 

    matenimientos : Mantenimientos[] ;  

    constructor(matenimientos : any[]) {
        this.matenimientos = matenimientos;
        this.datosgrupo = {...matenimientos[0]}; 

        for(let i = 1; i < matenimientos.length; i++) {
            this.datosgrupo.CodigoImtEquipo = this.datosgrupo.CodigoImtEquipo + ',' + String(matenimientos[i].CodigoImtEquipo);

        }

    }

}



