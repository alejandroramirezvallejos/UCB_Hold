import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';
import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';

@Component({
  selector: 'app-prestamos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './prestamos-crear.component.html',
  styleUrl: './prestamos-crear.component.css'
})
export class PrestamosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  prestamo: Prestamos = {
    Id: 0,
    CarnetUsuario: null,
    NombreUsuario: null,
    ApellidoPaternoUsuario: null,
    TelefonoUsuario: null,
    NombreGrupoEquipo: null,
    CodigoImt: null,
    FechaSolicitud: null,
    FechaPrestamoEsperada: null,
    FechaPrestamo: null,
    FechaDevolucionEsperada: null,
    FechaDevolucion: null,
    Observacion: null,
    EstadoPrestamo: null
  };

  constructor(private prestamoapi: PrestamosAPIService) {}
  registrar() {
    // TODO: El servicio actual está diseñado para carrito, no para CRUD administrativo
    // Se necesita implementar un método crearPrestamoAdmin en el servicio
    alert('Funcionalidad de crear préstamo pendiente de implementar en el backend');
    this.cerrar();
    
    /* Una vez implementado el método en el servicio, usar:
    this.prestamoapi.crearPrestamoAdmin(this.prestamo).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al crear préstamo: ' + error);
        this.cerrar();
      }
    );
    */
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
