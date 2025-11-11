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
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';

@Component({
  selector: 'app-prestamos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , VercontratoComponent, PantallaCargaComponent , VistaPrestamosComponent , AvisoEliminarComponent , MostrarerrorComponent , Aviso ,AvisoExitoComponent , BuscadorComponent ],
  templateUrl: './prestamos-tabla.component.html',
  styleUrls: ['./prestamos-tabla.component.css']
})
export class PrestamosTablaComponent extends Tabla implements OnInit {

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

  override columnas: string[] = ['Usuario','Carnet','Teléfono','Equipos','Fecha Solicitud','Fecha Préstamo Esperada','Fecha Devolución Esperada'];


  
  // Propiedades para el filtro
  showEstados: boolean = false;
  estadoSeleccionado: string = '';
  estadosDisponibles: string[] = [ "atrasado", 'pendiente', 'rechazado', 'aprobado', 'activo', 'finalizado', 'cancelado'];

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
    this.prestamos.clear();
    
    if (datos.length === 0){
      this.prestamoscopia = new Map(this.prestamos); 
      return;
    } 
    
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





// --------------------- ELIMINACION ----------------------- // 

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


  aplicarFiltros(event?: [string, string]) {
    // Convertir el Map a un array de [key, value]
    let prestamosFiltrados = Array.from(this.prestamoscopia.entries());

    // Aplicar filtro de búsqueda si existe
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      prestamosFiltrados = prestamosFiltrados.filter(([_, prestamo]) => {
        switch (event[1]) {
          case 'Usuario':
            return this.normalizeText(prestamo.datosgrupo.NombreUsuario || '').includes(busquedaNormalizada) ||
                  this.normalizeText(prestamo.datosgrupo.ApellidoPaternoUsuario || '').includes(busquedaNormalizada);
          case 'Carnet':
            return this.normalizeText(prestamo.datosgrupo.CarnetUsuario || '').includes(busquedaNormalizada);
          case 'Teléfono':
            return this.normalizeText(prestamo.datosgrupo.TelefonoUsuario || '').includes(busquedaNormalizada);
          case 'Equipos':
            return this.normalizeText(prestamo.datosgrupo.NombreGrupoEquipo || '').includes(busquedaNormalizada);
          case 'Fecha Solicitud':
            return this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaSolicitud)).includes(busquedaNormalizada);
          case 'Fecha Préstamo Esperada':
              return this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaPrestamoEsperada)).includes(busquedaNormalizada);
          case 'Fecha Devolución Esperada':
            return this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaDevolucionEsperada)).includes(busquedaNormalizada);
          default:  // 'Todas las columnas'
           return this.normalizeText(prestamo.datosgrupo.NombreUsuario || '').includes(busquedaNormalizada) ||
               this.normalizeText(prestamo.datosgrupo.ApellidoPaternoUsuario || '').includes(busquedaNormalizada) ||
               this.normalizeText(prestamo.datosgrupo.CarnetUsuario || '').includes(busquedaNormalizada) ||
               this.normalizeText(prestamo.datosgrupo.NombreGrupoEquipo || '').includes(busquedaNormalizada) ||
               this.normalizeText(prestamo.datosgrupo.CodigoImt || '').includes(busquedaNormalizada) ||
               this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaSolicitud)).includes(busquedaNormalizada) ||
               this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaPrestamoEsperada)).includes(busquedaNormalizada) ||
               this.normalizeText(this.formatDate(prestamo.datosgrupo.FechaDevolucionEsperada)).includes(busquedaNormalizada);
        }
      }); 
    }

    // Aplicar filtro de estado si existe
     if (this.estadoSeleccionado !== '') {
        const buscado = this.estadoSeleccionado.toLowerCase();
        prestamosFiltrados = prestamosFiltrados.filter(([_, prestamo]) =>
            this.getEstadoCalculado(prestamo).toLowerCase() === buscado
        );
    }

    // Reconstruir el Map con los resultados filtrados
    this.prestamos = new Map<number, PrestamoAgrupados>(prestamosFiltrados);
  }

  

  limpiarFiltros() {

    this.estadoSeleccionado = '';
    this.prestamos = new Map(this.prestamoscopia);
    this.showEstados = false;
  }

  getEstadoCalculado(prestamo: PrestamoAgrupados): string {
    const estadoOrig = (prestamo?.datosgrupo?.EstadoPrestamo || '').toLowerCase();
    if (!prestamo?.datosgrupo) return estadoOrig;

    const fechaDev = prestamo.datosgrupo.FechaDevolucionEsperada;

    if (!fechaDev) return estadoOrig;

    // Trabajar con las fechas en UTC para comparación consistente
    // Crear una copia de la fecha
    const fechaDevCopy = new Date(fechaDev.getTime());
    
    // Establecer a fin de día en UTC (equivalente a 23:59:59 Bolivia)
    fechaDevCopy.setUTCHours(23, 59, 59, 999);
    
    // Comparar con ahora en UTC
    const ahora = new Date();

    if ((estadoOrig === 'activo' || estadoOrig === "aprobado") && fechaDevCopy < ahora) {
      return 'atrasado';
    }
    
    return estadoOrig;
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
