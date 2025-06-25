import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MueblesCrearComponent } from '../muebles-crear/muebles-crear.component';
import { MueblesEditarComponent } from '../muebles-editar/muebles-editar.component';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';

@Component({
  selector: 'app-muebles-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MueblesCrearComponent, MueblesEditarComponent],
  templateUrl: './muebles-tabla.component.html',
  styleUrl: './muebles-tabla.component.css'
})
export class MueblesTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  muebles: Muebles[] = [];
  mueblesFiltrados: Muebles[] = [];

  muebleSeleccionado: Muebles = {
    Id: 0,
    Nombre: '',
    NumeroGaveteros: 0,
    Ubicacion: '',
    Tipo: '',
    Costo: 0,
    Longitud: 0,
    Profundidad: 0,
    Altura: 0
  };

  terminoBusqueda: string = '';


  constructor(private muebleapi: MuebleService) { }

  ngOnInit() {
    this.cargarMuebles();
  }

  limpiarMuebleSeleccionado() {
    this.muebleSeleccionado = {
      Id: 0,
      Nombre: '',
      NumeroGaveteros: 0,
      Ubicacion: '',
      Tipo: '',
      Costo: 0,
      Longitud: 0,
      Profundidad: 0,
      Altura: 0
    };
  }

  crearmueble() {
    this.botoncrear.set(true);
  }

  cargarMuebles() {
    this.muebleapi.obtenerMuebles().subscribe(
      (data: Muebles[]) => {
        this.muebles = data;
        this.mueblesFiltrados = [...this.muebles];
        this.aplicarBusqueda();
      },
      (error) => {
        console.error('Error al cargar los muebles:', error);
      }
    );
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
    this.muebleapi.eliminarMueble(this.muebleSeleccionado.Id).subscribe(
      (response) => {
        this.cargarMuebles();
      },
      (error) => {
        alert('Error al eliminar el mueble: ' + error);
      }
    );
    this.limpiarMuebleSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarMuebleSeleccionado();
  }

  
}
