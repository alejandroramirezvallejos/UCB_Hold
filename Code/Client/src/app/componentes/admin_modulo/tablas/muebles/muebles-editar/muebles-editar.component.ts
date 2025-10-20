import { Component, EventEmitter, Input, Output, signal, WritableSignal, OnChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-muebles-editar',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './muebles-editar.component.html',
  styleUrl: './muebles-editar.component.css'
})
export class MueblesEditarComponent extends BaseTablaComponent implements OnChanges {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() muebleOriginal: Muebles = new Muebles();

  mueble: Muebles = { ...this.muebleOriginal };

  constructor(private muebleapi: MuebleService) { 
    super(); 
  }

  ngOnChanges() {
    this.mueble = { ...this.muebleOriginal };
  }

  validaredicion(){
    this.mensajeaviso="¿Desea confirmar los cambios realizados al mueble?";
    this.aviso.set(true);
  }

  confirmar() {
    this.muebleapi.actualizarMueble(this.mueble).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = "Mueble editado exitosamente.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror = "Error al editar el mueble.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
