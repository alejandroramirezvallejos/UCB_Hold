import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';

@Component({
  selector: 'app-equipos-editar',
  imports: [FormsModule],
  templateUrl: './equipos-editar.component.html',
  styleUrl: './equipos-editar.component.css'
})
export class EquiposEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() equipo : Equipos ={
    Id: 0,
    Nombre: '',
    Modelo: '',
    Marca: '',
    NombreGrupoEquipo: '',
    CodigoImt: 0,
    CodigoUcb: '',
    NumeroSerial: '',
    EstadoEquipo: '',
    Ubicacion: '',
    NombreGavetero: '',
    CostoReferencia: 0,
    Descripcion: '',
    TiempoMaximoPrestamo: 0,
    Procedencia: ''
  };

  constructor(private equipoapi: EquipoService) {}; 

  


  confirmar (){
 
    this.equipoapi.editarEquipo(this.equipo).subscribe({
      next: (response) => {
        alert( response);
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.message);
        this.cerrar();
      }
        });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
