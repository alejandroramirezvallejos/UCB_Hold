import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';

@Component({
  selector: 'app-accesorios-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './accesorios-crear.component.html',
  styleUrl: './accesorios-crear.component.css'
})
export class AccesoriosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();


  accesorio : Accesorio = {
    id: 0,
    nombre: '',
    modelo: '',
    tipo: '',
    descripcion: '',
    codigo_imt: '',
    precio: null,
    url_data_sheet: '',
    nombreEquipoAsociado: ''
  };


  constructor(private accesorioapi : AccesoriosService){}; 

  // TODO : implementar
  registrar(){

    this.accesorioapi.crearAccesorio(this.accesorio).subscribe(
      response => {
        this.Actualizar.emit(); 
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    );
   
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
