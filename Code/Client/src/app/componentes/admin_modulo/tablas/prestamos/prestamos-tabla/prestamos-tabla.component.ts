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
  prestamos: PrestamoAgrupados[] = [];
  prestamoscopia: PrestamoAgrupados[] = [];

  vercontrato : WritableSignal<boolean> = signal(false);

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
        this.agruparPrestamos(data);
      },
      (error) => {
        console.error('Error al cargar los préstamos:', error);
      }
    );
  }

 agruparPrestamos(datos: Prestamos[]) {
    this.prestamos = []; 
    
    if (datos.length === 0) return;

    let prestamosarray: Prestamos[] = [];
    
    for (let i = 0; i < datos.length; i++) {
        prestamosarray.push(datos[i]); 
        
        if (i === datos.length - 1 || datos[i].Id !== datos[i + 1]?.Id) {
            this.prestamos.push(new PrestamoAgrupados(prestamosarray));
            prestamosarray = []; 
        }
    }
    this.prestamoscopia = [...this.prestamos]; 


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
    // Comenzar con la copia completa
    let prestamosFiltrados = [...this.prestamoscopia];

    // Aplicar filtro de búsqueda si existe
    if (this.terminoBusqueda.trim() !== '') {
      prestamosFiltrados = prestamosFiltrados.filter(prestamo =>
        (prestamo.datosgrupo.NombreUsuario|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (prestamo.datosgrupo.ApellidoPaternoUsuario|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (prestamo.datosgrupo.CarnetUsuario|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (prestamo.datosgrupo.NombreGrupoEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (prestamo.datosgrupo.CodigoImt || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (prestamo.datosgrupo.EstadoPrestamo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
      );
    }

    // Aplicar filtro de estado si existe
    if (this.estadoSeleccionado !== '') {
      prestamosFiltrados = prestamosFiltrados.filter(prestamo =>
        (prestamo.datosgrupo.EstadoPrestamo || '').toLowerCase() === this.estadoSeleccionado.toLowerCase()
      );
    }

    this.prestamos = prestamosFiltrados;
  }

  limpiarFiltros() {
    this.terminoBusqueda = '';
    this.estadoSeleccionado = '';
    this.prestamos = [...this.prestamoscopia];
    this.showEstados = false;  }

    

  aprobarprestamo(indice : number) {
    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos[indice].datosgrupo.Id, 'aprobado').subscribe({
      next: (response) => {
        this.cargarPrestamos(); 
      },
      error: (error) => {
        alert(error.error.error + ':' + error.error.mensaje);
      }
    });

  }

  rechazarprestamo(indice : number) {
    
    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos[indice].datosgrupo.Id, 'rechazado').subscribe({
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
