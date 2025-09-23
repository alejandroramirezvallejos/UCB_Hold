import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';

import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';
import { PrestamoAgrupados } from '../../../../../models/PrestamoAgrupados';
import { VercontratoComponent } from '../vercontrato/vercontrato.component';

@Component({
  selector: 'app-prestamos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , VercontratoComponent],
  templateUrl: './prestamos-tabla.component.html',
  styleUrls: ['./prestamos-tabla.component.css']
})
export class PrestamosTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  prestamos:  Map<number, PrestamoAgrupados>= new Map<number, PrestamoAgrupados>();
  prestamoscopia: Map<number, PrestamoAgrupados> = new Map<number, PrestamoAgrupados>();

  vercontrato : WritableSignal<boolean> = signal(false);

  prestamoSeleccionado: Prestamos =  new Prestamos();

  terminoBusqueda: string = '';
  
  // Propiedades para el filtro
  showEstados: boolean = false;
  estadoSeleccionado: string = '';
  estadosDisponibles: string[] = ['pendiente', 'rechazado', 'aprobado', 'activo', 'finalizado', 'cancelado'];

  hover = {
    filter: false
  };

  constructor(private prestamosapi: PrestamosAPIService ) {}



  ngOnInit() {
    this.cargarPrestamos();
  }

  limpiarPrestamoSeleccionado() {
    this.prestamoSeleccionado = new Prestamos();
  }

  crearprestamo() {
    this.botoncrear.set(true);
  }

  cargarPrestamos() {
    this.prestamosapi.obtenerPrestamos().subscribe({
      next :(data: Prestamos[]) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        console.error('Error al cargar los préstamos:', error);
      }
    });
  }

 agruparPrestamos(datos: Prestamos[]) {
    this.prestamos = new Map<number, PrestamoAgrupados>(); 
    
    if (datos.length === 0) return;
    
   for (const prestamo of datos) {
        if (prestamo.Id == null) continue;

        if (!this.prestamos.has(prestamo.Id)) {
            this.prestamos.set(prestamo.Id, new PrestamoAgrupados([prestamo]));
        } 
        else {
            this.prestamos.get(prestamo.Id)!.insertarEquipo(prestamo);
        }
    }

    this.prestamoscopia = new Map(this.prestamos);


}



  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.aplicarFiltros();
  }


// --------------------- ELIMINACION -----------------------
  eliminarPrestamo(prestamo: Prestamos) {
    this.prestamoSeleccionado = prestamo;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.prestamosapi.eliminarPrestamo(this.prestamoSeleccionado.Id).subscribe({
      next: (response) => {
        this.cargarPrestamos();
  
      },
      error: (error) => {
        alert('Error al eliminar el préstamo: ' + error);
      }
    });
    this.limpiarPrestamoSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarPrestamoSeleccionado();
  }

//-----------------------------------------------------




  mostrarEstados() {
    this.showEstados = !this.showEstados;
  }

  seleccionarEstado(estado: string) {
    this.estadoSeleccionado = estado;
    this.showEstados = false;
    this.aplicarFiltros();
  }

  aplicarFiltros() {
    // Convertir el Map a un array de [key, value]
    let prestamosFiltrados = Array.from(this.prestamoscopia.entries());

    // Aplicar filtro de búsqueda si existe
    if (this.terminoBusqueda.trim() !== '') {
        prestamosFiltrados = prestamosFiltrados.filter(([_, prestamo]) =>
            (prestamo.datosgrupo.NombreUsuario || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
            (prestamo.datosgrupo.ApellidoPaternoUsuario || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
            (prestamo.datosgrupo.CarnetUsuario || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
            (prestamo.datosgrupo.NombreGrupoEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
            (prestamo.datosgrupo.CodigoImt || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
            (prestamo.datosgrupo.EstadoPrestamo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
        );
    }

    // Aplicar filtro de estado si existe
    if (this.estadoSeleccionado !== '') {
        prestamosFiltrados = prestamosFiltrados.filter(([_, prestamo]) =>
            (prestamo.datosgrupo.EstadoPrestamo || '').toLowerCase() === this.estadoSeleccionado.toLowerCase()
        );
    }

    // Reconstruir el Map con los resultados filtrados
    this.prestamos = new Map<number, PrestamoAgrupados>(prestamosFiltrados);
  }

  limpiarFiltros() {
    this.terminoBusqueda = '';
    this.estadoSeleccionado = '';
    this.prestamos = new Map(this.prestamoscopia);
    this.showEstados = false;
  }

  aprobarprestamo(key : number) {

    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos.get(key)!.datosgrupo.Id, 'aprobado').subscribe({
      next: (response) => {
        this.cargarPrestamos(); 
      },
      error: (error) => {
        alert(error.error.error + ':' + error.error.mensaje);
      }
    });

  }

  rechazarprestamo(key : number) {

    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos.get(key)!.datosgrupo.Id, 'rechazado').subscribe({
      next: (response) => {
        this.cargarPrestamos(); 
      },
      error: (error) => {
        alert(error.error.error + ':' + error.error.mensaje);
      }
    });

  }

  cambiarestadovercontrato(prestamo : Prestamos) {
  this.prestamoSeleccionado = prestamo;
  this.vercontrato.set(!this.vercontrato());
  }


}
