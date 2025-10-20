import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Categorias } from '../../../../../models/admin/Categorias';
import { CategoriasCrearComponent } from '../categorias-crear/categorias-crear.component';
import { CategoriasEditarComponent } from '../categorias-editar/categorias-editar.component';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-categorias-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoriasCrearComponent, CategoriasEditarComponent, AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent],
  templateUrl: './categorias-tabla.component.html',
  styleUrl: './categorias-tabla.component.css'
})
export class CategoriasTablaComponent extends BaseTablaComponent {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  categorias: Categorias[] = [];
  categoriascopia: Categorias[] = [];

  categoriaSeleccionada: Categorias = new Categorias();

  terminoBusqueda: string = '';

  

  constructor(private categoriaService: CategoriaService) {
    super();
  }

  ngOnInit() {
    this.cargarCategorias();
  }

  limpiarCategoriaSeleccionada() {
    this.categoriaSeleccionada = new Categorias();
  }

  crearCategoria() {
    this.botoncrear.set(true);
  }

  cargarCategorias() {
    this.categoriaService.obtenercategorias().subscribe(
      (data: any[]) => {
        this.categorias = data; 
        this.categoriascopia = [...this.categorias];
      },
      (error) => {
        this.mensajeerror = 'Error al cargar las categorías , intente mas tarde ';
        console.error('Error al cargar las categorías:', error.message);
        this.error.set(true);
      }
    );
  }

  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    this.categorias = this.categoriascopia.filter(categoria =>
      categoria.Nombre?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.categorias = [...this.categoriascopia];
  }

  editarCategoria(categoria: Categorias) {
    this.botoncrear.set(false);
    this.categoriaSeleccionada = { ...categoria };
    this.botoneditar.set(true);
  }

  eliminarCategoria(categoria: Categorias) {
    this.categoriaSeleccionada = categoria;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    if (this.categoriaSeleccionada.Id) {
      this.categoriaService.eliminarCategoria(this.categoriaSeleccionada.Id).subscribe({
        next: (response) => {
          this.cargarCategorias();
          this.mensajeexito="Categoría eliminada con éxito";
          this.exito.set(true);
        },
        error: (error) => {
          this.mensajeerror="Error al eliminar la categoría , intente mas tarde";
          alert('Error al eliminar la categoría: ' + error.message);
          this.error.set(true);
        }
      });
    }
    this.limpiarCategoriaSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarCategoriaSeleccionada();
  }



}
