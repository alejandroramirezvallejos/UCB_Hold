import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-muebles-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './muebles-crear.component.html',
  styleUrl: './muebles-crear.component.css'
})
export class MueblesCrearComponent extends BaseTablaComponent{

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  mueble: Muebles = new Muebles();

  constructor(private muebleapi: MuebleService) { 
    super();
  }

  validarcreacion(){
    this.mensajeaviso="¿Desea crear el mueble "+ this.mueble.Nombre+"?";
    this.aviso.set(true);
  }

  registrar() {
    this.muebleapi.crearMueble(this.mueble).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito="Mueble "+ this.mueble.Nombre+" creado exitosamente";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror="Error al crear el mueble , Intente mas tarde";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
