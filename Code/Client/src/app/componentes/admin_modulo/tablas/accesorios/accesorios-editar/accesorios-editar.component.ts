import { Component, Input, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';

@Component({
  selector: 'app-accesorios-editar',
  imports: [FormsModule],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css'
})
export class AccesoriosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
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
  confirmar (){
    console.log('Confirmando edición de accesorio:', this.accesorio);
    // Aquí iría la lógica para enviar al backend
    this.cerrar();
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
