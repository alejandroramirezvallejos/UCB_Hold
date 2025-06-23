import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';

@Component({
  selector: 'app-gaveteros-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './gaveteros-crear.component.html',
  styleUrl: './gaveteros-crear.component.css'
})
export class GaveterosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();


  gavetero : Gaveteros = {
    Id: 0,
    Nombre: '',
    Tipo: '',
    NombreMueble: '',
    Longitud: null,
    Profundidad: null,
    Altura: null
  };


  constructor(private gaveteroapi : GaveteroService){}; 

  // TODO : implementar
  registrar(){

    this.gaveteroapi.crearGavetero(this.gavetero).subscribe(
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
