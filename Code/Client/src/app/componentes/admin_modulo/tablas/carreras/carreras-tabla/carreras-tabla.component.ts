import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Carrera } from '../../../../../models/admin/Carreras';
import { CarrerasCrearComponent } from '../carreras-crear/carreras-crear.component';
import { CarrerasEditarComponent } from '../carreras-editar/carreras-editar.component';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';

@Component({
  selector: 'app-carreras-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, CarrerasCrearComponent, CarrerasEditarComponent],
  templateUrl: './carreras-tabla.component.html',
  styleUrl: './carreras-tabla.component.css'
})
export class CarrerasTablaComponent  {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  carreras: Carrera[] = [];
  carrerascopia: Carrera[] = [];

  carreraSeleccionada: Carrera = {
    Id: 0,
    Nombre: ''
  };

  terminoBusqueda: string = '';

 

  constructor(private carreraService: CarreraService) {}

  ngOnInit() {
    this.cargarCarreras();
  }

  limpiarCarreraSeleccionada() {
    this.carreraSeleccionada = {
      Id: 0,
      Nombre: ''
    };
  }

  crearCarrera() {
    this.botoncrear.set(true);
  }

  cargarCarreras() {
    this.carreraService.obtenerCarreras().subscribe(
      (data: any[]) => {
        this.carreras = data.map(item => ({
          Id: item.id,
          Nombre: item.nombre
        }));
        this.carrerascopia = [...this.carreras];
      },
      (error) => {
        console.error('Error al cargar las carreras:', error);
      }
    );
  }

  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    this.carreras = this.carrerascopia.filter(carrera =>
      carrera.Nombre?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.carreras = [...this.carrerascopia];
  }

  editarCarrera(carrera: Carrera) {
    this.botoncrear.set(false);
    this.carreraSeleccionada = { ...carrera };
    this.botoneditar.set(true);
  }

  eliminarCarrera(carrera: Carrera) {
    this.carreraSeleccionada = carrera;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    if (this.carreraSeleccionada.Id) {
      this.carreraService.eliminarCarrera(this.carreraSeleccionada.Id).subscribe(
        (response) => {
          this.cargarCarreras();
        },
        (error) => {
          alert('Error al eliminar la carrera: ' + error);
        }
      );
    }
    this.limpiarCarreraSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarCarreraSeleccionada();
  }

 

  
}
