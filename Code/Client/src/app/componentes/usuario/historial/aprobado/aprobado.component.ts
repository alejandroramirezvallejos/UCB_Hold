import { Component, signal, WritableSignal } from '@angular/core';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { Aviso } from '../aviso/aviso.component';

@Component({
  selector: 'app-aprobado',
  standalone: true,
  imports: [CommonModule, Aviso],
  templateUrl: './aprobado.component.html',
  styleUrl: './aprobado.component.css'
})
export class AprobadoComponent {
  datos  = new Map<number, PrestamoAgrupados>;
  avisocancelar : WritableSignal<boolean> = signal(false);
  avisoaprobar : WritableSignal<boolean> = signal(false);
  itemSeleccionado : Prestamos | null = null;

  constructor( private usuario : UsuarioService , private prestamoApi : PrestamosAPIService){}; 


  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , 'aprobado').subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 
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
  avisocancelarf(item : Prestamos) {
    this.avisocancelar.set(!this.avisocancelar());
    this.itemSeleccionado = item;
  }

  avisorecogerf(item : Prestamos){
     this.avisoaprobar.set(!this.avisoaprobar());
     this.itemSeleccionado = item;

  }

  recoger() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'activo').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.avisoaprobar.set(false);
      }, 
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cancelar() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'cancelado').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.avisocancelar.set(false);
      }, 
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }
    });
  }


}
