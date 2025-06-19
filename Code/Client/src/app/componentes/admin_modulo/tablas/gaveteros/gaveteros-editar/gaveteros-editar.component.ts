import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Gaveteros } from '../../../../../models/admin/Gaveteros';
import { GaveteroService } from '../../../../../services/APIS/Gavetero/gavetero.service';

@Component({
  selector: 'app-gaveteros-editar',
  imports: [FormsModule],
  templateUrl: './gaveteros-editar.component.html',
  styleUrl: './gaveteros-editar.component.css'
})
export class GaveterosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() gavetero : Gaveteros ={
    Id: 0,
    Nombre: '',
    Tipo: '',
    NombreMueble: '',
    Longitud: 0,
    Profundidad: 0,
    Altura: 0
  };

  constructor(private gaveteroapi: GaveteroService) {}; 

  


  confirmar (){
 
    this.gaveteroapi.editarGavetero(this.gavetero).subscribe({
      next: (response) => {
        alert( response);
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
