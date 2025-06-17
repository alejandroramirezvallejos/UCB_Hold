import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GruposEquiposCrearComponent } from '../grupos-equipos-crear/grupos-equipos-crear.component';
import { GruposEquiposEditarComponent } from '../grupos-equipos-editar/grupos-equipos-editar.component';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';

@Component({
  selector: 'app-grupos-equipos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, GruposEquiposCrearComponent, GruposEquiposEditarComponent],
  templateUrl: './grupos-equipos-tabla.component.html',
  styleUrl: './grupos-equipos-tabla.component.css'
})
export class GruposEquiposTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  gruposEquipos: GrupoEquipo[] = [];
  gruposEquiposFiltrados: GrupoEquipo[] = [];

  grupoEquipoSeleccionado: GrupoEquipo = {
    id: 0,
    nombre: '',
    modelo: '',
    marca: '',
    nombreCategoria: '',
    descripcion: '',
    url_data_sheet: '',
    link: ''
  };

  terminoBusqueda: string = '';
  sortColumn: string = 'nombre';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private grupoequipoapi: GrupoequipoService) { }

  ngOnInit() {
    this.cargarGruposEquipos();
  }

  limpiarGrupoEquipoSeleccionado() {
    this.grupoEquipoSeleccionado = {
      id: 0,
      nombre: '',
      modelo: '',
      marca: '',
      nombreCategoria: '',
      descripcion: '',
      url_data_sheet: '',
      link: ''
    };
  }

  creargrupoequipo() {
    this.botoncrear.set(true);
  }

  cargarGruposEquipos() {
    this.grupoequipoapi.getGrupoEquipo('','').subscribe(
      (data: GrupoEquipo[]) => {
        this.gruposEquipos = data;
        this.gruposEquiposFiltrados = [...this.gruposEquipos];
        this.aplicarBusqueda();
      },
      (error) => {
        console.error('Error al cargar los grupos de equipos:', error);
      }
    );
  }

  buscar() {
    this.aplicarBusqueda();
  }

  aplicarBusqueda() {
    if (this.terminoBusqueda.trim() === '') {
      this.gruposEquiposFiltrados = [...this.gruposEquipos];
    } else {
      this.gruposEquiposFiltrados = this.gruposEquipos.filter(grupoequipo =>
        grupoequipo.nombre.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (grupoequipo.modelo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (grupoequipo.marca || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (grupoequipo.nombreCategoria || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (grupoequipo.descripcion || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
      );
    }
    this.aplicarOrdenamiento();
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.aplicarBusqueda();
  }

  editarGrupoEquipo(grupoequipo: GrupoEquipo) {
    this.botoncrear.set(false);
    this.grupoEquipoSeleccionado = { ...grupoequipo };
    this.botoneditar.set(true);
  }

  eliminarGrupoEquipo(grupoequipo: GrupoEquipo) {
    this.grupoEquipoSeleccionado = grupoequipo;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.grupoequipoapi.eliminarGrupoEquipo(this.grupoEquipoSeleccionado.id).subscribe(
      (response) => {
        this.cargarGruposEquipos();
      },
      (error) => {
        alert('Error al eliminar el grupo de equipo: ' + error);
      }
    );
    this.limpiarGrupoEquipoSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarGrupoEquipoSeleccionado();
  }

  aplicarOrdenamiento() {
    this.gruposEquiposFiltrados.sort((a, b) => {
      const valorA = (a as any)[this.sortColumn];
      const valorB = (b as any)[this.sortColumn];

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

    this.aplicarOrdenamiento();
  }
}
