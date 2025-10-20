import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Carrera } from '../../../../../models/admin/Carreras';
import { CarrerasCrearComponent } from '../carreras-crear/carreras-crear.component';
import { CarrerasEditarComponent } from '../carreras-editar/carreras-editar.component';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-carreras-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, CarrerasCrearComponent, CarrerasEditarComponent , AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent],
  templateUrl: './carreras-tabla.component.html',
  styleUrl: './carreras-tabla.component.css'
})
export class CarrerasTablaComponent extends BaseTablaComponent {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  carreras: Carrera[] = [];
  carrerascopia: Carrera[] = [];

  carreraSeleccionada: Carrera = new Carrera();

  terminoBusqueda: string = '';

 

  constructor(private carreraService: CarreraService) {
    super();
  }

  ngOnInit() {
    this.cargarCarreras();
  }

  limpiarCarreraSeleccionada() {
    this.carreraSeleccionada = new Carrera();
  }

  crearCarrera() {
    this.botoncrear.set(true);
  }

  cargarCarreras() {
    this.carreraService.obtenerCarreras().subscribe({
      next: (data: any[]) => {
        this.carreras = data.map(item => ({
          Id: item.id,
          Nombre: item.nombre
        }));
        this.carrerascopia = [...this.carreras];
      },
      error: (error) => {
        this.mensajeerror="Error al cargar las carreras , intente mas tarde";
        console.error('Error al cargar las carreras:', error);
        this.error.set(true);
      }
    });
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
      this.carreraService.eliminarCarrera(this.carreraSeleccionada.Id).subscribe({
        next: (response) => {
          this.cargarCarreras();
          this.mensajeexito="Carrera eliminada correctamente";
          this.exito.set(true);
        },
        error: (error) => {
          this.mensajeerror="Error al eliminar la carrera";
          console.error('Error al eliminar la carrera: ' + error);
          this.error.set(true);
        }
      });
    }
    this.limpiarCarreraSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarCarreraSeleccionada();
  }

 

  
}
