import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';

@Component({
  selector: 'app-equipos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './equipos-crear.component.html',
  styleUrl: './equipos-crear.component.css'
})
export class EquiposCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();


  equipo : Equipos = {
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


  constructor(private equipoapi : EquipoService){}; 

  // TODO : implementar
  registrar(){

    this.equipoapi.crearEquipo(this.equipo).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
       });
   
  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
