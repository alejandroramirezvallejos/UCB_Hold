import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Carrera } from '../../../../../models/admin/Carreras';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-carreras-editar',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './carreras-editar.component.html',
  styleUrl: './carreras-editar.component.css'
})
export class CarrerasEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() carrera: Carrera = new Carrera();

  constructor(private carreraService: CarreraService) {
    super();
  }

  validaredicion(){
    if (!this.carrera.Nombre || this.carrera.Nombre.trim() === '') {
          this.mensajeerror="El nombre de la carrera no puede estar vacío.";
          this.error.set(true);
          return;
        }

    this.mensajeaviso="¿Está seguro de que desea  editar esta carrera?";
    this.aviso.set(true);

  }

  confirmar() {
   
    this.carreraService.actualizarCarrera(this.carrera).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito="Carrera editada con éxito.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror="error al actualizar la carrera";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
