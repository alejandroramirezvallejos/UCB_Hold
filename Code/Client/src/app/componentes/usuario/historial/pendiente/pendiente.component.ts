import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';
import { Aviso } from '../aviso/aviso.component';

@Component({
  selector: 'app-pendiente',
  imports: [CommonModule , Aviso],
  templateUrl: './pendiente.component.html',
  styleUrl: './pendiente.component.css'
})
export class PendienteComponent {
 datos  = new Map<number, PrestamoAgrupados>;

  itemSeleccionado: Prestamos | null = null;
  avisocancelar : WritableSignal<boolean> = signal(false);

  constructor( private usuario : UsuarioService , private prestamoApi : PrestamosAPIService){}; 


  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , 'pendiente').subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 
    }
  
  aviso(item : Prestamos) {
    this.itemSeleccionado = item;
    this.avisocancelar.set(!this.avisocancelar());

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
