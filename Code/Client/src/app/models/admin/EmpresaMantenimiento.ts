import { Basemodel } from "../base/Basemodel";

export class EmpresaMantenimiento extends Basemodel {
    NombreEmpresa?: string | null;
    NombreResponsable?: string | null;
    ApellidoResponsable?: string | null;
    Telefono?: string | null;
    Nit?: string | null;
    Direccion?: string | null;

    constructor(){
        super();
        this.NombreEmpresa = null;
        this.NombreResponsable = null;
        this.ApellidoResponsable = null;
        this.Telefono = null;
        this.Nit = null;
        this.Direccion = null;
    }


    
}