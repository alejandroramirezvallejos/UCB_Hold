import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Categorias } from '../../../../../models/admin/Categorias';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';

@Component({
  selector: 'app-categorias-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './categorias-editar.component.html',
  styleUrl: './categorias-editar.component.css'
})
export class CategoriasEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() categoria: Categorias = {
    Id: 0,
    Nombre: ''
  };

  constructor(private categoriaService: CategoriaService) {}

  confirmar() {
    if (!this.categoria.Nombre || this.categoria.Nombre.trim() === '') {
      alert('Por favor ingrese el nombre de la categoría');
      return;
    }

    this.categoriaService.actualizarCategoria(this.categoria).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
