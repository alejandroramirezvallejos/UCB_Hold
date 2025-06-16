import { Component, Input, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';

@Component({
  selector: 'app-prestamos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './prestamos-crear.component.html',
  styleUrl: './prestamos-crear.component.css'
})
export class PrestamosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);

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
    Observacion: null,    EstadoPrestamo: null
  };

  // TODO : implementar
  registrar() {
    console.log('Registrando préstamo:', this.prestamo);
    // Aquí iría la lógica para enviar al backend
    this.cerrar();
  }

  cerrar() {
    this.botoncrear.set(false);
  }

  // Método para formatear fecha para input datetime-local
  formatearFechaParaInput(fecha: Date | null): string {
    if (!fecha) return '';
    const date = new Date(fecha);
    return date.toISOString().slice(0, 16);
  }
  // Método para convertir string de input a Date
  convertirStringAFecha(fechaString: string): Date | null {
    return fechaString ? new Date(fechaString) : null;
  }

  // Método para manejar cambio de fecha
  onFechaChange(event: Event, campo: string) {
    const target = event.target as HTMLInputElement;
    const fecha = target.value ? new Date(target.value) : null;
    
    switch(campo) {
      case 'FechaSolicitud':
        this.prestamo.FechaSolicitud = fecha;
        break;
      case 'FechaPrestamoEsperada':
        this.prestamo.FechaPrestamoEsperada = fecha;
        break;
      case 'FechaDevolucionEsperada':
        this.prestamo.FechaDevolucionEsperada = fecha;
        break;
    }
  }

  // Establecer fecha actual para fecha de solicitud
  establecerFechaActual() {
    this.prestamo.FechaSolicitud = new Date();
  }
}
