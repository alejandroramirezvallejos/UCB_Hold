import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output} from '@angular/core';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-buscador',
  imports: [FormsModule, CommonModule],
  templateUrl: './buscador.component.html',
  styleUrl: './buscador.component.css'
})
export class BuscadorComponent {
  @Input()columnas!: string[];
  @Output() terminoBusqueda: EventEmitter<[string , string]> = new EventEmitter<[string , string]>();
  @Output() onToggle: EventEmitter<boolean> = new EventEmitter<boolean>();
  terminoBusquedaLocal: string = '';
  columnaSeleccionada: string = '';
  mostrarColumnas : boolean = false;
  isHovered: boolean = false;
  buscar() {
    this.terminoBusqueda.emit([this.terminoBusquedaLocal, this.columnaSeleccionada]);
  }
  limpiarBusqueda() {
    this.terminoBusquedaLocal = '';
    this.terminoBusqueda.emit(['','']);
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
    this.columnaSeleccionada = columna;
    this.mostrarColumnas = false;
  }
}
