import { signal, WritableSignal } from "@angular/core";
import { Prestamos } from "../../../../models/admin/Prestamos";
import { PrestamoAgrupados } from "../../../../models/PrestamoAgrupados";
import { PrestamosAPIService } from "../../../../services/APIS/prestamo/prestamos-api.service";
import { UsuarioService } from "../../../../services/usuario/usuario.service";

export abstract class HistorialBase {
datos  = new Map<number, PrestamoAgrupados>;
itemSeleccionado: Prestamos | null = null;

prestamosVista : Prestamos[] = [];

abrirVistaPrestamos : boolean = false;

error : WritableSignal<boolean> = signal (false) ;
mensajeerror : string = "" ;

exito : WritableSignal<boolean> = signal (false) ;
mensajeexito : string = "" ;


protected abstract estado : string ; 


constructor(protected prestamoApi: PrestamosAPIService, protected usuario: UsuarioService) {};

cargarDatos() {
    if(this.usuario.vacio()==false){
     this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , this.estado).subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los prestamos , intente mas tarde";
        console.error( error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
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

AbrirVista(item: Prestamos[]) {
  this.prestamosVista = item;
  this.abrirVistaPrestamos = true;
}

cerrarVista() {
  this.abrirVistaPrestamos = false;
  this.prestamosVista = [];
} 




}