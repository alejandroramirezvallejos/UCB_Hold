import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';

import { CategoriaService } from '../../../services/APIS/Categoria/categoria.service';
import { Categorias } from '../../../models/admin/Categorias';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule, ListaObjetosComponent, FormsModule , MostrarerrorComponent],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css',
})
export class PantallaMainComponent {
  showCategories = false;
  solicitud: string = '';
  categoriasSeleccionadas: Set<string> = new Set();
  items: Categorias[] = [];
  hover: {
    search: boolean;
    categories: boolean;
    clear: boolean;
  } = {
    search: false,
    categories: false,
    clear: false,
  };

  error : WritableSignal<boolean> = signal(false);
  mensajeerror : string = "";

  constructor(private categorias: CategoriaService) {}

  ngOnInit(): void {
    this.categorias.obtenercategorias().subscribe({
      next: (data) => (this.items = data),
      error: (error) =>{
        this.mensajeerror = "Error al cargar las categorias , intente mas tarde"; 
        console.error('Error en componente:' + error)
        this.error.set(true);
      },
    });
  }

  limpiar() {
    this.solicitud = '';
    this.categoriasSeleccionadas.clear();
    this.hover.clear = false;
  }

  mostrarcategorias() {
    this.showCategories = !this.showCategories;
  }

  seleccionarcategoria(categoria: string) {
    if (this.categoriasSeleccionadas.has(categoria)) {
      this.categoriasSeleccionadas.delete(categoria);
    } else {
      if (categoria === '') {
        this.categoriasSeleccionadas.clear();
      } else {
        this.categoriasSeleccionadas.delete('');
      }
      this.categoriasSeleccionadas.add(categoria);
    }
  }

  estaCategoriaSeleccionada(categoria: string): boolean {
    return this.categoriasSeleccionadas.has(categoria);
  }

  get categoriasArray(): string[] {
    return Array.from(this.categoriasSeleccionadas);
  }
}
