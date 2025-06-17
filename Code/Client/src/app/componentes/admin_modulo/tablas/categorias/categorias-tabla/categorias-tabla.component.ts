import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Categorias } from '../../../../../models/admin/Categorias';
import { CategoriasCrearComponent } from '../categorias-crear/categorias-crear.component';
import { CategoriasEditarComponent } from '../categorias-editar/categorias-editar.component';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';

@Component({
  selector: 'app-categorias-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoriasCrearComponent, CategoriasEditarComponent],
  templateUrl: './categorias-tabla.component.html',
  styleUrl: './categorias-tabla.component.css'
})
export class CategoriasTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  categorias: Categorias[] = [];
  categoriascopia: Categorias[] = [];

  categoriaSeleccionada: Categorias = {
    Id: 0,
    Nombre: ''
  };

  terminoBusqueda: string = '';

  // Sorting properties
  sortColumn: string = 'Nombre';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private categoriaService: CategoriaService) {}

  ngOnInit() {
    this.cargarCategorias();
  }

  limpiarCategoriaSeleccionada() {
    this.categoriaSeleccionada = {
      Id: 0,
      Nombre: ''
    };
  }

  crearCategoria() {
    this.botoncrear.set(true);
  }

  cargarCategorias() {
    this.categoriaService.obtenercategorias().subscribe(
      (data: any[]) => {
        this.categorias = data.map(item => ({
          Id: item.id,
          Nombre: item.nombre
        }));
        this.categoriascopia = [...this.categorias];
      },
      (error) => {
        console.error('Error al cargar las categorías:', error);
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
      this.categoriaService.eliminarCategoria(this.categoriaSeleccionada.Id).subscribe(
        (response) => {
          this.cargarCategorias();
        },
        (error) => {
          alert('Error al eliminar la categoría: ' + error);
        }
      );
    }
    this.limpiarCategoriaSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarCategoriaSeleccionada();
  }

  aplicarOrdenamiento() {
    this.categorias.sort((a, b) => {
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
