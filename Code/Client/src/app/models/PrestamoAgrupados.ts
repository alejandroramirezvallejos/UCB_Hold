import { Prestamos } from "./admin/Prestamos";
export class PrestamoAgrupados {
    datosgrupo : Prestamos ; 

    equipos : Prestamos[] ;  

    constructor(equipos : any[] ) {
        this.equipos = equipos;
        this.datosgrupo = {...equipos[0]}; 

        for(let i = 1; i < equipos.length; i++) {
            this.datosgrupo.NombreGrupoEquipo = this.datosgrupo.NombreGrupoEquipo + ' , ' + equipos[i].NombreGrupoEquipo;
            this.datosgrupo.CodigoImt = this.datosgrupo.CodigoImt + ' , ' + equipos[i].CodigoImt;
        }

    }

    insertarEquipo(equipo: Prestamos) {
        this.equipos.push(equipo);
        this.datosgrupo.NombreGrupoEquipo = this.datosgrupo.NombreGrupoEquipo + ' , ' + equipo.NombreGrupoEquipo;
        this.datosgrupo.CodigoImt = this.datosgrupo.CodigoImt + ' , ' + equipo.CodigoImt;
    }


}