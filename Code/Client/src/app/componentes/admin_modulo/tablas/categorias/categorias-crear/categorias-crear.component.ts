import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';
import { Categorias } from '../../../../../models/admin/Categorias';

@Component({
  selector: 'app-categorias-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './categorias-crear.component.html',
  styleUrl: './categorias-crear.component.css'
})
export class CategoriasCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  nombreCategoria: string = '';

  constructor(private categoriaService: CategoriaService) {}

  registrar() {
    if (this.nombreCategoria.trim() === '') {
      alert('Por favor ingrese el nombre de la categoría');
      return;
    }

    const categoria: Categorias = {
      Nombre: this.nombreCategoria
    };

    this.categoriaService.crearCategoria(categoria).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.nombreCategoria = '';
    this.botoncrear.set(false);
  }
}
