export class EmpresaMantenimiento {
    Id?: number;
    NombreEmpresa?: string | null;
    NombreResponsable?: string | null;
    ApellidoResponsable?: string | null;
    Telefono?: string | null;
    Nit?: string | null;
    Direccion?: string | null;

    constructor(){
        this.Id = 0;
        this.NombreEmpresa = null;
        this.NombreResponsable = null;
        this.ApellidoResponsable = null;
        this.Telefono = null;
        this.Nit = null;
        this.Direccion = null;
    }


    
}