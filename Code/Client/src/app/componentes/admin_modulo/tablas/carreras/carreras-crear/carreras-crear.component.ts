import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';

@Component({
  selector: 'app-carreras-crear',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent , AvisoExitoComponent , Aviso],
  templateUrl: './carreras-crear.component.html',
  styleUrl: './carreras-crear.component.css'
})
export class CarrerasCrearComponent extends BaseTablaComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  nombreCarrera: string = '';

  constructor(private carreraService: CarreraService) {
    super();
  }

  validarregistro(){
     if (this.nombreCarrera.trim() === '') {
      this.mensajeerror= "El nombre de la carrera no puede estar vacío.";
      this.error.set(true);
      return;
    }

    this.mensajeaviso="¿Está seguro de que desea crear la carrera "+ this.nombreCarrera + "?";
    this.aviso.set(true);

  }

  registrar() {
  
    this.carreraService.crearCarrera(this.nombreCarrera).subscribe({
      next : (response) => {
        this.Actualizar.emit();
        this.mensajeexito= "Carrera creada exitosamente.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror= "Error al crear la carrera.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.nombreCarrera = '';
    this.botoncrear.set(false);
  }
}
