import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';

@Component({
  selector: 'app-accesorios-editar',
  imports: [FormsModule],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css'
})
export class AccesoriosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() accesorio : Accesorio ={
    id: 0,
    nombre: '',
    modelo: '',
    tipo: '',
    descripcion: '',
    codigo_imt: '',
    precio: 0,
    url_data_sheet: '',
    nombreEquipoAsociado: ''
  };

  constructor(private accesorioapi: AccesoriosService) {}; 

  


  confirmar (){
 
    this.accesorioapi.editarAccesorio(this.accesorio).subscribe(
      response => {
        alert( response);
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al editar accesorio: ' + error.message);
        this.cerrar();
      }
    );
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
