import { CommonModule } from '@angular/common';
import { Component, EventEmitter, input, Output } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';

import { CategoriaService } from '../../../services/APIS/Categoria/categoria.service';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule, ListaObjetosComponent, FormsModule],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css',
})
export class PantallaMainComponent {
  showCategories = false;
  solicitud: string = '';
  categoria: string = '';
  enviar: boolean = false;
  items: any[] = [];
  hover: {
    search: boolean;
    categories: boolean;
    clear: boolean;
  } = {
    search: false,
    categories: false,
    clear: false,
  };

  constructor(
    private categorias: CategoriaService
  ) {

  }

  ngOnInit(): void {
    this.categorias.obtenercategorias().subscribe({
      next: (data) => (this.items = data),
      error: (error) => alert('Error en componente:' + error),
    });
  }

  limpiar(){
    this.solicitud = '';
    this.categoria = '';
    this.hover.clear = false;
    this.enviar = !this.enviar;
  }


  mostrarcategorias() {
    this.showCategories = !this.showCategories;
  }

  seleccionarcategoria(categoria: string) {
    if (categoria == 'sin categoria') {
      this.categoria = '';
    } else {
      this.categoria = categoria;
    }
    this.mostrarcategorias();
    this.actualizarobjetos();
  }

 
  actualizarobjetos() {
   this.enviar = !this.enviar;
  }




}
