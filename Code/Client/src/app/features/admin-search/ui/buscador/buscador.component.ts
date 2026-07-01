import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminTableSort } from '@shared/lib/admin-table';

@Component({
  selector: 'app-buscador',
  imports: [FormsModule, CommonModule],
  templateUrl: './buscador.component.html',
  styleUrl: './buscador.component.css',
})
export class BuscadorComponent {
  @Input() columnas: string[] = [];
  @Output() terminoBusqueda = new EventEmitter<[string, string]>();
  @Output() onToggle = new EventEmitter<boolean>();
  @Output() sortChange = new EventEmitter<AdminTableSort>();

  terminoBusquedaLocal = '';
  columnaSeleccionada = '';
  mostrarColumnas = false;
  isHovered = false;
  sortCol = '';
  sortDir: 'asc' | 'desc' = 'asc';

  buscar() {
    this.terminoBusqueda.emit([
      this.terminoBusquedaLocal,
      this.columnaSeleccionada,
    ]);
  }

  onSearchInput() {
    this.buscar();
  }

  limpiarBusqueda() {
    this.terminoBusquedaLocal = '';
    this.columnaSeleccionada = '';
    this.terminoBusqueda.emit(['', '']);
    this.mostrarColumnas = false;
  }

  toggleColumnas() {
    this.mostrarColumnas = !this.mostrarColumnas;
    this.onToggle.emit(this.mostrarColumnas);
  }

  cerrarColumnas() {
    this.mostrarColumnas = false;
  }
  onMouseEnter() {
    this.isHovered = true;
  }
  onMouseLeave() {
    this.isHovered = false;
  }

  seleccionarColumna(columna: string) {
    const columnaOrdenable = columna.trim();

    if (this.sortCol === columnaOrdenable)
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    else {
      this.sortCol = columnaOrdenable;
      this.sortDir = 'asc';
    }
    this.columnaSeleccionada = columnaOrdenable;
    this.mostrarColumnas = false;

    this.sortChange.emit({ col: this.sortCol, dir: this.sortDir });
  }

  iconoOrdenColumna(columna: string): string {
    if (this.sortCol !== columna.trim()) return 'fa-sort';

    return this.sortDir === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }
}
