import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';
import { PrestamosCrearComponent } from '../prestamos-crear/prestamos-crear.component';
import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';

@Component({
  selector: 'app-prestamos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PrestamosCrearComponent],
  templateUrl: './prestamos-tabla.component.html',
  styleUrls: ['./prestamos-tabla.component.css']
})
export class PrestamosTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
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

  sortColumn: string = 'NombreUsuario';
  sortDirection: 'asc' | 'desc' = 'asc';

  ngOnInit() {
    this.cargarPrestamos();
  }

  limpiarPrestamoSeleccionado() {
    this.prestamoSeleccionado = {
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
  }

  crearprestamo() {
    this.botoncrear.set(true);
  }

  cargarPrestamos() {
    this.prestamosapi.obtenerPrestamos().subscribe(
      (data: Prestamos[]) => {
        this.prestamos = data;
        this.prestamoscopia = [...this.prestamos];
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

    this.prestamos = this.prestamoscopia.filter(prestamo =>
      prestamo.NombreUsuario?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      prestamo.ApellidoPaternoUsuario?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      prestamo.CarnetUsuario?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      prestamo.NombreGrupoEquipo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      prestamo.CodigoImt?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      prestamo.EstadoPrestamo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.prestamos = [...this.prestamoscopia];
  }



  eliminarPrestamo(prestamo: Prestamos) {
    this.prestamoSeleccionado = prestamo;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.prestamosapi.eliminarPrestamo(this.prestamoSeleccionado.Id).subscribe(
      (response) => {
        this.cargarPrestamos();
      },
      (error) => {
        alert('Error al eliminar el préstamo: ' + error);
      }
    );
    this.limpiarPrestamoSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarPrestamoSeleccionado();
  }

  // Aplicar ordenamiento
  aplicarOrdenamiento() {
    this.prestamos.sort((a, b) => {
      // Type assertion para acceso dinámico
      const valorA = (a as any)[this.sortColumn];
      const valorB = (b as any)[this.sortColumn];

      // Convertir a minúsculas si son strings
      let compA = typeof valorA === 'string' ? valorA.toLowerCase() : valorA;
      let compB = typeof valorB === 'string' ? valorB.toLowerCase() : valorB;

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
}
