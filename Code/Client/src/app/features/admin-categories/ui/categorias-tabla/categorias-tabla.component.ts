import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Categorias } from '@entities/admin';
import { CategoriasCrearComponent } from '../categorias-crear/categorias-crear.component';
import { CategoriasEditarComponent } from '../categorias-editar/categorias-editar.component';
import { CategoriaService } from '@entities/category';
import { AvisoEliminarComponent } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { Tabla } from '@shared/lib/admin-table';
import { BuscadorComponent } from '@features/admin-search';
import { extractErrorMessage } from '@shared/lib/error';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-categorias-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    CategoriasCrearComponent,
    CategoriasEditarComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    AuditPanelComponent,
  ],
  templateUrl: './categorias-tabla.component.html',
  styleUrl: './categorias-tabla.component.css',
})
export class CategoriasTablaComponent extends Tabla {
  expandedRowId: number | null = null;
  auditRefresh = 0;

  toggleExpand(id: number): void {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  categorias: Categorias[] = [];
  categoriascopia: Categorias[] = [];
  categoriaSeleccionada: Categorias = new Categorias();

  constructor(private readonly categoriaService: CategoriaService) {
    super();
  }

  ngOnInit(): void {
    this.cargarCategorias();
  }

  limpiarCategoriaSeleccionada(): void {
    this.categoriaSeleccionada = new Categorias();
  }

  crearCategoria(): void {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }

  cargarCategorias(): void {
    this.categoriaService.obtenercategorias().subscribe({
      next: (data: Categorias[]) => {
        this.categorias = data;
        this.categoriascopia = [...this.categorias];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar las categorías, intente más tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  aplicarFiltros(event?: [string, string]): void {
    if (!event || event[0].trim() === '') {
      this.limpiarBusqueda();
      return;
    }
    const busquedaNormalizada = this.normalizeText(event[0]);
    this.categorias = this.categoriascopia.filter((categoria) =>
      this.normalizeText(categoria.Nombre || '').includes(busquedaNormalizada),
    );
  }

  limpiarBusqueda(): void {
    this.categorias = [...this.categoriascopia];
  }

  editarCategoria(categoria: Categorias): void {
    this.botoncrear.set(false);
    this.categoriaSeleccionada = { ...categoria };
    this.botoneditar.set(true);
  }

  eliminarCategoria(categoria: Categorias): void {
    this.categoriaSeleccionada = categoria;
    this.alertaeliminar = true;
  }

  confirmarEliminacion(): void {
    if (this.categoriaSeleccionada.Id) {
      this.categoriaService
        .eliminarCategoria(this.categoriaSeleccionada.Id)
        .subscribe({
          next: () => {
            this.cargarCategorias();
            this.mensajeexito = 'Categoría eliminada con éxito';
            this.exito.set(true);
            this.auditRefresh++;
          },
          error: (error) => {
            const errorMsg = extractErrorMessage(
              error,
              'Error al eliminar la categoría, intente más tarde',
            );
            this.mensajeerror = errorMsg;
            this.error.set(true);
          },
        });
    }
    this.limpiarCategoriaSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion(): void {
    this.alertaeliminar = false;
    this.limpiarCategoriaSeleccionada();
  }
}
