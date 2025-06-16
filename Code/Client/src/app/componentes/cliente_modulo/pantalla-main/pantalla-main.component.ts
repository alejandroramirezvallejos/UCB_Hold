import { CommonModule } from '@angular/common';
import { Component, EventEmitter, input, Output } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';
import { BuscadorService } from '../../../services/buscador/buscador.service';
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
    microfono: boolean;
    scanner: boolean;
    categories: boolean;
  } = {
    search: false,
    microfono: false,
    scanner: false,
    categories: false,
  };

  constructor(
    private buscador: BuscadorService,
    private categorias: CategoriaService
  ) {
    this.solicitud = buscador.producto;
    this.categoria = buscador.categoria;
  }

  ngOnInit(): void {
    this.categorias.obtenercategorias().subscribe({
      next: (data) => (this.items = data),
      error: (error) => alert('Error en componente:' + error),
    });

    
  }

  // Añade este método
  toggleCategories() {
    this.showCategories = !this.showCategories;
  }

  selectCategory(categoria: string) {
    if (categoria == 'sin categoria') {
      this.categoria = '';
    } else {
      this.categoria = categoria;
    }
    this.toggleCategories();
    this.submitRequest();
  }

  //TODO : Optimizar esto seguro que es mala practica
  submitRequest() {
    // Actualiza el servicio con los datos actuales
    this.buscador.producto = this.solicitud;
    this.buscador.categoria = this.categoria;
    this.enviar = false;
    setTimeout(() => {
      this.enviar = true;
    }, 0);
  }

  trackByName(index: number, item: { name: string }): string {
    return item.name;
  }
}
