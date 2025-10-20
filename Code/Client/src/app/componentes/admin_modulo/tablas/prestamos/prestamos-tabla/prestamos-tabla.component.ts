import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Prestamos } from '../../../../../models/admin/Prestamos';

import { PrestamosAPIService } from '../../../../../services/APIS/prestamo/prestamos-api.service';
import { PrestamoAgrupados } from '../../../../../models/PrestamoAgrupados';
import { VercontratoComponent } from '../vercontrato/vercontrato.component';

import { finalize } from 'rxjs';
import { VistaPrestamosComponent } from '../../../../vista-prestamos/vista-prestamos.component';
import { PantallaCargaComponent } from '../../../../pantallas_avisos/pantalla-carga/pantalla-carga.component';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-prestamos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , VercontratoComponent, PantallaCargaComponent , VistaPrestamosComponent , AvisoEliminarComponent , MostrarerrorComponent , Aviso ,AvisoExitoComponent ],
  templateUrl: './prestamos-tabla.component.html',
  styleUrls: ['./prestamos-tabla.component.css']
})
export class PrestamosTablaComponent extends BaseTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  cargando : boolean = false;

  alertaeliminar: boolean = false;
  prestamos:  Map<number, PrestamoAgrupados>= new Map<number, PrestamoAgrupados>();
  prestamoscopia: Map<number, PrestamoAgrupados> = new Map<number, PrestamoAgrupados>();

  vercontrato : WritableSignal<boolean> = signal(false);

  prestamoSeleccionado: Prestamos =  new Prestamos();
  prestamoKeySeleccionado: number = 0;


  avisorechazar : WritableSignal<boolean> = signal(false);
  mensajeavisorechazar : string = "¿Está seguro de rechazar el préstamo seleccionado?";


  terminoBusqueda: string = '';
  
  // Propiedades para el filtro
  showEstados: boolean = false;
  estadoSeleccionado: string = '';
  estadosDisponibles: string[] = ['pendiente', 'rechazado', 'aprobado', 'activo', 'finalizado', 'cancelado'];

  hover = {
    filter: false
  };


  abrirVista : boolean = false;

  prestamosVista : Prestamos[] = [];


  constructor(private prestamosapi: PrestamosAPIService ) {
    super();
  }



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
        this.seleccionarEstado(this.estadoSeleccionado);
      },
      error: (error) => {
        this.mensajeerror = 'Error al cargar los préstamos. Por favor, inténtelo de nuevo más tarde.';
        console.error('Error al cargar los préstamos:', error);
        this.error.set(true);
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
    this.cargando=true; 
    this.prestamosapi.eliminarPrestamo(this.prestamoSeleccionado.Id)
    .pipe(finalize(() => this.cargando = false))
    .subscribe({
      next: (response) => {
        this.mensajeexito = 'Préstamo eliminado con éxito.';
        this.exito.set(true);
        this.cargarPrestamos();
        

      },
      error: (error) => {
        this.mensajeerror = 'Error al eliminar el préstamo. Por favor, inténtelo de nuevo más tarde.';
        console.error('Error al eliminar el préstamo: ' + error);
        this.error.set(true);
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


  validaraprobacion(key : number){
    this.mensajeaviso = "¿Está seguro de aprobar el préstamo seleccionado?";
    this.prestamoKeySeleccionado = key;
    this.aviso.set(true);
  }
  
 

  aprobarprestamo(key : number) {

    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos.get(key)!.datosgrupo.Id, 'aprobado').subscribe({
      next: (response) => {
        this.mensajeexito = 'Préstamo aprobado con éxito.';
        this.exito.set(true);
        this.cargarPrestamos(); 
      },
      error: (error) => {
        this.mensajeerror = 'Error al aprobar el préstamo. Por favor, inténtelo de nuevo más tarde.';
        console.error(error.error.error + ':' + error.error.mensaje);
        this.error.set(true);
      }
    });
    this.prestamoKeySeleccionado = 0;

  }


  validarrechazo(key : number){
    this.mensajeavisorechazar = "¿Está seguro de rechazar el préstamo seleccionado?";
    this.prestamoKeySeleccionado = key;
    this.avisorechazar.set(true);
  }

  rechazarprestamo(key : number) {

    this.prestamosapi.cambiarEstadoPrestamo(this.prestamos.get(key)!.datosgrupo.Id, 'rechazado').subscribe({
      next: (response) => {
        this.mensajeexito = 'Préstamo rechazado con éxito.';
        this.exito.set(true);
        this.cargarPrestamos(); 
       
      },
      error: (error) => {
        this.mensajeerror = 'Error al rechazar el préstamo. Por favor, inténtelo de nuevo más tarde.';
        console.error(error.error.error + ':' + error.error.mensaje);
        this.error.set(true);
      }
    });

    this.prestamoKeySeleccionado = 0;

  }

  cambiarestadovercontrato(prestamo : Prestamos) {
    this.prestamoSeleccionado = prestamo;
    this.vercontrato.set(!this.vercontrato());
  }


  // Vista prestamos
  abrirVistaPrestamos(prestamos : Prestamos[]) {
    this.prestamosVista = prestamos;
    this.abrirVista = true;
  }

  cerrarVistaPrestamos() {
    this.abrirVista = false;
    this.prestamosVista = [];
  }


}
