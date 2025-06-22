import { Component, signal, WritableSignal } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule, DatePipe } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { Aviso } from '../aviso/aviso.component';

@Component({
  selector: 'app-activo',
  imports: [CommonModule, DatePipe , Aviso],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css'
})
export class ActivoComponent {
  datos  = new Map<number, PrestamoAgrupados>;

  itemSeleccionado: Prestamos | null = null;
  aviso : WritableSignal<boolean> = signal(false);

  constructor( private usuario : UsuarioService , private prestamoApi : PrestamosAPIService){}; 


  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , 'activo').subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 
}

  avisoDevolver(item : Prestamos) {
    this.itemSeleccionado = item;
    this.aviso.set(!this.aviso());
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

  finalizado() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'finalizado').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.aviso.set(!this.aviso());
        this.itemSeleccionado = null;
      }, 
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }
    });
  }




}
