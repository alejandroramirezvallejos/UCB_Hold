import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-buscador',
  imports: [FormsModule, CommonModule],
  templateUrl: './buscador.component.html',
  styleUrl: './buscador.component.css',
})
export class BuscadorComponent {
  @Output() terminoBusqueda = new EventEmitter<[string, string]>();

  terminoBusquedaLocal = '';
  columnaSeleccionada = '';
  isHovered = false;

  buscar(): void {
    this.terminoBusqueda.emit([
      this.terminoBusquedaLocal,
      this.columnaSeleccionada,
    ]);
  }

  onSearchInput(): void {
    this.buscar();
  }

  limpiarBusqueda(): void {
    this.terminoBusquedaLocal = '';
    this.columnaSeleccionada = '';
    this.terminoBusqueda.emit(['', '']);
  }

  onMouseEnter(): void {
    this.isHovered = true;
  }

  onMouseLeave(): void {
    this.isHovered = false;
  }
}
