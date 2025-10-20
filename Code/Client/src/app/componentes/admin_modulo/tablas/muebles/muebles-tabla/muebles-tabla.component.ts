import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MueblesCrearComponent } from '../muebles-crear/muebles-crear.component';
import { MueblesEditarComponent } from '../muebles-editar/muebles-editar.component';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-muebles-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MueblesCrearComponent, MueblesEditarComponent,AvisoEliminarComponent, MostrarerrorComponent, AvisoExitoComponent],
  templateUrl: './muebles-tabla.component.html',
  styleUrl: './muebles-tabla.component.css'
})
export class MueblesTablaComponent extends BaseTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  muebles: Muebles[] = [];
  mueblesFiltrados: Muebles[] = [];

  muebleSeleccionado: Muebles = new Muebles() ;

  terminoBusqueda: string = '';


  constructor(private muebleapi: MuebleService) { 
    super(); 
  }

  ngOnInit() {
    this.cargarMuebles();
  }

  limpiarMuebleSeleccionado() {
    this.muebleSeleccionado = new Muebles();
  }

  crearmueble() {
    this.botoncrear.set(true);
  }

  cargarMuebles() {
    this.muebleapi.obtenerMuebles().subscribe({
      next: (data: Muebles[]) => {
        this.muebles = data;
        this.mueblesFiltrados = [...this.muebles];
        this.aplicarBusqueda();
      },
      error: (error) => {
        this.mensajeerror = 'Error al cargar los muebles. Intente más tarde.';
        console.error('Error al cargar los muebles:', error);
        this.error.set(true);
      }
    });
  }

  buscar() {
    this.aplicarBusqueda();
  }

  aplicarBusqueda() {
    if (this.terminoBusqueda.trim() === '') {
      this.mueblesFiltrados = [...this.muebles];
    } else {
      this.mueblesFiltrados = this.muebles.filter(mueble =>
        (mueble.Nombre || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mueble.Tipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mueble.Ubicacion || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        String(mueble.Costo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        String(mueble.NumeroGaveteros || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
      );
    }

  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.aplicarBusqueda();
  }

  editarMueble(mueble: Muebles) {
    this.botoncrear.set(false);
    this.muebleSeleccionado = { ...mueble };
    this.botoneditar.set(true);
  }

  eliminarMueble(mueble: Muebles) {
    this.muebleSeleccionado = mueble;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.muebleapi.eliminarMueble(this.muebleSeleccionado.Id).subscribe({
      next: (response) => {
        this.mensajeexito = 'Mueble eliminado exitosamente.';
        this.exito.set(true);
        this.cargarMuebles();
      },
      error: (error) => {
        this.mensajeerror = 'Error al eliminar el mueble. Intente más tarde.';
        console.error('Error al eliminar el mueble: ' + error);
        this.error.set(true);
      }
    });
    this.limpiarMuebleSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarMuebleSeleccionado();
  }

  
}
