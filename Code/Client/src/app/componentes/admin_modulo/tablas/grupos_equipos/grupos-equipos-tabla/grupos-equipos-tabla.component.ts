import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GruposEquiposCrearComponent } from '../grupos-equipos-crear/grupos-equipos-crear.component';
import { GruposEquiposEditarComponent } from '../grupos-equipos-editar/grupos-equipos-editar.component';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';

@Component({
  selector: 'app-grupos-equipos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, GruposEquiposCrearComponent, GruposEquiposEditarComponent,AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent, BuscadorComponent],
  templateUrl: './grupos-equipos-tabla.component.html',
  styleUrl: './grupos-equipos-tabla.component.css'
})
export class GruposEquiposTablaComponent extends Tabla implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  gruposEquipos: GrupoEquipo[] = [];
  gruposEquiposFiltrados: GrupoEquipo[] = [];

  categorias: string[] = []; 

  grupoEquipoSeleccionado: GrupoEquipo = new GrupoEquipo();

  override columnas: string[] = ['Nombre','Cantidad','Modelo','Marca','Categoría','Descripción'];

  constructor(private grupoequipoapi: GrupoequipoService , private categoriasAPI : CategoriaService) { 
    super();
  }

  ngOnInit() {
    this.cargarGruposEquipos();
    this.obtenerCategorias();
  }

  obtenerCategorias() {
    this.categoriasAPI.obtenercategorias().subscribe({
      next : (data) => {
        this.categorias = data.map(categoria => categoria.Nombre);
      },
      error: (error) => {
        this.mensajeerror="Error al cargar las categorías, intente más tarde";
        console.error('Error al cargar las categorías:', error.error.mensaje);
        this.error.set(true);
      }
  });
  }

  limpiarGrupoEquipoSeleccionado() {
    this.grupoEquipoSeleccionado = new GrupoEquipo();
  }

  creargrupoequipo() {
    this.botoncrear.set(true);
  }

  cargarGruposEquipos() {
    this.grupoequipoapi.getGrupoEquipo('','').subscribe({
      next :(data: GrupoEquipo[]) => {
        this.gruposEquipos = data;
        this.gruposEquiposFiltrados = [...this.gruposEquipos];
        this.aplicarFiltros();
      },
      error: (error) => {
        this.mensajeerror="Error al cargar los grupos de equipos, intente más tarde";
        console.error('Error al cargar los grupos de equipos:', error);
        this.error.set(true);
      }
    });
  }

  
  buscar() {
    this.aplicarFiltros();
  }

  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.gruposEquiposFiltrados = this.gruposEquipos.filter(grupoequipo => {
        switch (event[1]) {
          case 'Nombre':
            return this.normalizeText(grupoequipo.nombre || '').includes(busquedaNormalizada);
          case 'Cantidad':
            return this.normalizeText(String(grupoequipo.Cantidad || '')).includes(busquedaNormalizada);
          case 'Modelo':
            return this.normalizeText(grupoequipo.modelo || '').includes(busquedaNormalizada);
          case 'Marca':
            return this.normalizeText(grupoequipo.marca || '').includes(busquedaNormalizada);
          case 'Categoría':
            return this.normalizeText(grupoequipo.nombreCategoria || '').includes(busquedaNormalizada);
          case 'Descripción':
            return this.normalizeText(grupoequipo.descripcion || '').includes(busquedaNormalizada);
          default:  // 'Todas las columnas'
            return this.normalizeText(grupoequipo.nombre || '').includes(busquedaNormalizada) ||
                  this.normalizeText(String(grupoequipo.Cantidad || '')).includes(busquedaNormalizada) ||
                  this.normalizeText(grupoequipo.modelo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(grupoequipo.marca || '').includes(busquedaNormalizada) ||
                  this.normalizeText(grupoequipo.nombreCategoria || '').includes(busquedaNormalizada) ||
                  this.normalizeText(grupoequipo.descripcion || '').includes(busquedaNormalizada);
        }
      });
    } else {
      // Crear una copia para evitar referencias
      this.gruposEquiposFiltrados = [...this.gruposEquipos];
    }
  }

  limpiarBusqueda() {

    this.aplicarFiltros();
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
    this.grupoequipoapi.eliminarGrupoEquipo(this.grupoEquipoSeleccionado.id).subscribe({
      next: (response) => {
        this.mensajeexito="Grupo de equipo eliminado exitosamente";
        this.exito.set(true);
        this.cargarGruposEquipos();
      },
      error: (error) => {
        this.mensajeerror="Error al eliminar el grupo de equipo, intente más tarde";
        console.error('Error al eliminar el grupo de equipo: ' + error);
        this.error.set(true);
      }}
    );
    this.limpiarGrupoEquipoSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarGrupoEquipoSeleccionado();
  }


}
