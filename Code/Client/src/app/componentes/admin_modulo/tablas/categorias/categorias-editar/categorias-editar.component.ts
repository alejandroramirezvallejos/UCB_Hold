import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Categorias } from '../../../../../models/admin/Categorias';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';

@Component({
  selector: 'app-categorias-editar',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent,AvisoExitoComponent,Aviso],
  templateUrl: './categorias-editar.component.html',
  styleUrl: './categorias-editar.component.css'
})
export class CategoriasEditarComponent  extends BaseTablaComponent{
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() categoria: Categorias = new Categorias();

  constructor(private categoriaService: CategoriaService) {
    super();
  }

  validaredicion(){
  if (!this.categoria.Nombre || this.categoria.Nombre.trim() === '') {
        this.mensajeerror = "Por favor ingrese el nombre de la categoría";
        this.error.set(true);
        return;
      }
      this.mensajeaviso="¿Está seguro de que desea actualizar la categoría?";
      this.aviso.set(true);

  }

  confirmar() {
   
    this.categoriaService.actualizarCategoria(this.categoria).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = "Categoría actualizada con éxito";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror = "No se pudo actualizar la Categoria intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
