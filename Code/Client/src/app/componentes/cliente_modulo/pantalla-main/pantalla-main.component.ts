import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';
import { CategoriaService } from '../../../services/APIS/Categoria/categoria.service';
import { Categorias } from '../../../models/admin/Categorias';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { extractErrorMessage } from '../../../utils/error-handler';
import { FiltrosService } from '../../../services/filtros.service';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule, ListaObjetosComponent, FormsModule , MostrarerrorComponent],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css',
})
export class PantallaMainComponent implements OnInit, OnDestroy {
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

  constructor(private categorias: CategoriaService, private filtrosService: FiltrosService, private el: ElementRef) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.showCategories && !this.el.nativeElement.contains(event.target)) {
      this.showCategories = false;
    }
  }

  ngOnInit(): void {
    this.categoriasSeleccionadas = this.filtrosService.categoriasSeleccionadas;
    this.solicitud = this.filtrosService.solicitud;

    this.categorias.obtenercategorias().subscribe({
      next: (data) => (this.items = data),
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar las categorias , intente mas tarde");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      },
    });
  }

  ngOnDestroy(): void {
    this.filtrosService.solicitud = this.solicitud;
  }

  limpiar() {
    this.solicitud = '';
    this.categoriasSeleccionadas.clear();
    this.filtrosService.limpiar();
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
