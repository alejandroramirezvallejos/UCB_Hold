import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoriaService } from '../../../../../services/APIS/Categoria/categoria.service';
import { Categorias } from '../../../../../models/admin/Categorias';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';

@Component({
  selector: 'app-categorias-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent, AvisoExitoComponent , Aviso],
  templateUrl: './categorias-crear.component.html',
  styleUrl: './categorias-crear.component.css'
})
export class CategoriasCrearComponent extends BaseTablaComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  nombreCategoria: string = '';

  constructor(private categoriaService: CategoriaService) {
    super();
  }

  validarregistro(){
  if (this.nombreCategoria.trim() === '') {
      this.mensajeerror= "el nombre de la categoria no puede estar vacia";
      this.error.set(true);
      return;
    }
    this.mensajeaviso="esta seguro de crear esta categoria?"
    this.aviso.set(true);


  }

  registrar() {
  
    const categoria: Categorias = {
      Id: 0, 
      Nombre: this.nombreCategoria
    };

    this.categoriaService.crearCategoria(categoria).subscribe({
      next:(response) => {
        this.Actualizar.emit();
        this.mensajeexito = "Categoria creada exitosamente";
        this.exito.set(true); 
      },
      error: (error) => {
        this.mensajeerror = "error al crear la categoria , intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
   });
  }

  cerrar() {
    this.nombreCategoria = '';
    this.botoncrear.set(false);
  }
}
