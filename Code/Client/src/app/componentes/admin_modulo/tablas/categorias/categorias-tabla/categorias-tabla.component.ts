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
import { Tabla } from '../../base/tabla';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { AuditPanelComponent } from "../../../audit-panel/audit-panel.component";
import { StickyScrollDirective } from '../../../../../directives/sticky-scroll.directive';
@Component({
  selector: 'app-categorias-tabla',
  standalone: true,
  imports: [StickyScrollDirective, CommonModule, FormsModule, CategoriasCrearComponent, CategoriasEditarComponent, AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent , BuscadorComponent, AuditPanelComponent],
  templateUrl: './categorias-tabla.component.html',
  styleUrl: './categorias-tabla.component.css'
})
export class CategoriasTablaComponent extends Tabla {
  expandedRowId: number | null = null;
  auditRefresh = 0;
  activeTab: 'tabla' | 'auditoria' = 'tabla';

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  categorias: Categorias[] = [];
  categoriascopia: Categorias[] = [];
  categoriaSeleccionada: Categorias = new Categorias();
  override columnas: string[] = [' Nombre'];
  constructor(private readonly categoriaService: CategoriaService) {
    super();
  }
  ngOnInit() {
    this.cargarCategorias();
  }
  limpiarCategoriaSeleccionada() {
    this.categoriaSeleccionada = new Categorias();
  }
  crearCategoria() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarCategorias() {
    this.categoriaService.obtenercategorias().subscribe({
      next: (data: any[]) => {
        this.categorias = data;
        this.categoriascopia = [...this.categorias];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al cargar las categorías, intente más tarde');
        this.mensajeerror = errorMsg;
        console.error('Error al cargar las categorías:', errorMsg);
        this.error.set(true);
      }
    });
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() === '') {
      this.limpiarBusqueda();
      return;
    }
    const busquedaNormalizada = this.normalizeText(event![0]);
    this.categorias = this.categoriascopia.filter(categoria =>
      this.normalizeText(categoria.Nombre || '').includes(busquedaNormalizada)
    );
  }
  limpiarBusqueda() {
    this.categorias = [...this.categoriascopia];
  }

  override sortTable(e: {col: string, dir: 'asc' | 'desc'}) {
    this.categorias = [...this.categorias].sort((a, b) => {
      const va = this.normalizeText(a.Nombre ?? '');
      const vb = this.normalizeText(b.Nombre ?? '');
      return e.dir === 'asc' ? va.localeCompare(vb) : vb.localeCompare(va);
    });
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
        this.auditRefresh++;
        },
        error: (error) => {
          const errorMsg = extractErrorMessage(error, "Error al eliminar la categoría, intente más tarde");
          this.mensajeerror = errorMsg;
          console.error('Error al eliminar la categoría:', errorMsg);
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
