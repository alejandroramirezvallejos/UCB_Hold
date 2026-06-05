import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-buscador',
  imports: [FormsModule, CommonModule],
  templateUrl: './buscador.component.html',
  styleUrl: './buscador.component.css'
})
export class BuscadorComponent {
  @Input() columnas!: string[];
  @Output() terminoBusqueda = new EventEmitter<[string, string]>();
  @Output() onToggle = new EventEmitter<boolean>();
  @Output() sortChange = new EventEmitter<{col: string, dir: 'asc' | 'desc'}>();

  terminoBusquedaLocal = '';
  columnaSeleccionada = '';
  mostrarColumnas = false;
  isHovered = false;
  sortCol = '';
  sortDir: 'asc' | 'desc' = 'asc';

  buscar() {
    this.terminoBusqueda.emit([this.terminoBusquedaLocal, this.columnaSeleccionada]);
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

  cerrarColumnas() { this.mostrarColumnas = false; }
  onMouseEnter() { this.isHovered = true; }
  onMouseLeave() { this.isHovered = false; }

  seleccionarColumna(columna: string) {
    if (this.sortCol === columna)
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    else {
      this.sortCol = columna;
      this.sortDir = 'asc';
    }
    this.columnaSeleccionada = columna;
    this.mostrarColumnas = false;
    // Only emit sort — do NOT call buscar() to avoid resetting array from copia
    this.sortChange.emit({ col: this.sortCol, dir: this.sortDir });
  }
}
