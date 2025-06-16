import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';
import { PrestamosCrearComponent } from '../prestamos-crear/prestamos-crear.component';
import { PrestamosEditarComponent } from '../prestamos-editar/prestamos-editar.component';
import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';

@Component({
  selector: 'app-prestamos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PrestamosCrearComponent, PrestamosEditarComponent],
  templateUrl: './prestamos-tabla.component.html',
  styleUrls: ['./prestamos-tabla.component.css']
})
export class PrestamosTablaComponent {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  valoreliminar: number = 0;
  prestamos: Prestamos[] = [];
  prestamoscopia: Prestamos[] = [];

  prestamoSeleccionado: Prestamos = {
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

  terminoBusqueda: string = '';

  constructor(private prestamosapi: PrestamosAPIService) {}

  // ----
  sortColumn: string = 'Id';
  sortDirection: 'asc' | 'desc' = 'asc';

  ngOnInit() {
    this.cargarPrestamos();
  }

  crearprestamo() {
    this.botoncrear.set(true);
  }
  cargarPrestamos() {
    // Simulación de carga de préstamos, en un caso real se haría una llamada a un servicio
    this.prestamosapi.obtenerPrestamos().subscribe(
      (data: any[]) => {
        this.prestamos = data;
        this.prestamoscopia = [...this.prestamos]; // Guardar una copia para la búsqueda
      },
      (error) => {
        console.error('Error al cargar los préstamos:', error);
      }
    );
  }

  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    this.prestamos = this.prestamos.filter(prestamo =>
      (prestamo.CarnetUsuario && prestamo.CarnetUsuario.toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      (prestamo.NombreUsuario && prestamo.NombreUsuario.toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      (prestamo.ApellidoPaternoUsuario && prestamo.ApellidoPaternoUsuario.toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      (prestamo.NombreGrupoEquipo && prestamo.NombreGrupoEquipo.toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      (prestamo.CodigoImt && prestamo.CodigoImt.toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      (prestamo.EstadoPrestamo && prestamo.EstadoPrestamo.toLowerCase().includes(this.terminoBusqueda.toLowerCase()))
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.prestamos = [...this.prestamoscopia];
  }

  editarPrestamo(prestamo: Prestamos) {
    this.botoncrear.set(false);
    this.prestamoSeleccionado = prestamo;
    this.botoneditar.set(true);
  }

  eliminarPrestamo(i: number) {
    this.valoreliminar = i;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.prestamos.splice(this.valoreliminar, 1);
    this.alertaeliminar = false;
    this.valoreliminar = 0;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.valoreliminar = 0;
  }

  // nose que hace
  aplicarOrdenamiento() {
    this.prestamos.sort((a, b) => {
      // Type assertion para acceso dinámico
      const valorA = (a as any)[this.sortColumn];
      const valorB = (b as any)[this.sortColumn];

      // Convertir a minúsculas si son strings
      let compA = typeof valorA === 'string' ? valorA?.toLowerCase() : valorA;
      let compB = typeof valorB === 'string' ? valorB?.toLowerCase() : valorB;

      if (compA < compB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      } else if (compA > compB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      } else {
        return 0;
      }
    });
  }

  ordenarPor(columna: string) {
    if (this.sortColumn === columna) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columna;
      this.sortDirection = 'asc';
    }

    this.aplicarOrdenamiento(); // Aplicar el ordenamiento
  }

  formatearFecha(fecha: Date | null): string {
    if (!fecha) return 'N/A';
    return new Date(fecha).toLocaleDateString('es-ES');
  }
}
