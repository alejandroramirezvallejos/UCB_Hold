import { Component, Input, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';

@Component({
  selector: 'app-prestamos-editar',
  imports: [FormsModule],
  templateUrl: './prestamos-editar.component.html',
  styleUrls: ['./prestamos-editar.component.css']
})
export class PrestamosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Input() prestamo: Prestamos = {
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

  confirmar() {
    console.log('Confirmando edición de préstamo:', this.prestamo);
    // Aquí iría la lógica para enviar al backend
    this.cerrar();
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
