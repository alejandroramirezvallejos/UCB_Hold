import { Prestamos } from "../../../../models/admin/Prestamos";
import { PrestamoAgrupados } from "../../../../models/PrestamoAgrupados";
import { PrestamosAPIService } from "../../../../services/APIS/prestamo/prestamos-api.service";
import { UsuarioService } from "../../../../services/usuario/usuario.service";

export abstract class HistorialBase {
datos  = new Map<number, PrestamoAgrupados>;
itemSeleccionado: Prestamos | null = null;


protected abstract estado : string ; 


constructor(protected prestamoApi: PrestamosAPIService, protected usuario: UsuarioService) {};

cargarDatos() {
    if(this.usuario.vacio()==false){
     this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , this.estado).subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 
    }
 
}

 agruparPrestamos(datos: Prestamos[]) {
    this.datos.clear();
    for (let prestamo of datos) {
      if( this.datos.has(prestamo.Id!)) {
        this.datos.get(prestamo.Id)!.insertarEquipo(prestamo);
      }
      else{
        this.datos.set(prestamo.Id! , new PrestamoAgrupados([prestamo]));
      }
    }

  }

  




}