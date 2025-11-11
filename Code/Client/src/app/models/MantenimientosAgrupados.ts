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

    constructor(matenimientos: any[]) {
    this.matenimientos = matenimientos;
    this.datosgrupo = {
      ...matenimientos[0],
      FechaMantenimiento: this.parseDateLocal(matenimientos[0].FechaMantenimiento),
      FechaFinalDeMantenimiento: this.parseDateLocal(matenimientos[0].FechaFinalDeMantenimiento),
    };

    for (let i = 1; i < matenimientos.length; i++) {
      this.datosgrupo.CodigoImtEquipo = this.datosgrupo.CodigoImtEquipo + ',' + String(matenimientos[i].CodigoImtEquipo);
    }
  }

  // Método para parsear fecha en zona local
  private parseDateLocal(dateString: string | null): Date | null {
    if (!dateString) return null;
    const [year, month, day] = dateString.split('T')[0].split('-').map(Number);  // Ignora la hora si existe
    return new Date(year, month - 1, day);  // Mes -1 porque Date usa 0-based
  }

}



