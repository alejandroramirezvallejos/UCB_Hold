import { Component, Input, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';

@Component({
  selector: 'app-accesorios-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './accesorios-crear.component.html',
  styleUrl: './accesorios-crear.component.css'
})
export class AccesoriosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);

  accesorio : Accesorio = {
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

  // TODO : implementar
  registrar(){
    console.log('Registrando accesorio:', this.accesorio);
    // Aquí iría la lógica para enviar al backend
    this.cerrar();
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
