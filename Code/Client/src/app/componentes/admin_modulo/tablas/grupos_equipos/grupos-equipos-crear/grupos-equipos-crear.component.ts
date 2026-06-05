import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { CustomSelectComponent } from '../../../../compartidos/custom-select/custom-select.component';
@Component({
  selector: 'app-grupos-equipos-crear',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent , Aviso , AvisoExitoComponent, CustomSelectComponent],
  templateUrl: './grupos-equipos-crear.component.html',
  styleUrl: './grupos-equipos-crear.component.css'
})
export class GruposEquiposCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Input() categorias: string[] = [];
  @Output() Actualizar = new EventEmitter<void>();
  grupoEquipo: GrupoEquipo = new GrupoEquipo();
  constructor(private grupoEquipoapi: GrupoequipoService) { 
    super(); 
  }
  validarregistro(){
    this.mensajeaviso="Desea registrar el nuevo grupo de equipo?";
    this.aviso.set(true);
  }
  registrar() {
    this.grupoEquipoapi.crearGrupoEquipo(this.grupoEquipo).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito="Grupo de equipo registrado exitosamente";
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al registrar el grupo de equipo");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar() {
    this.botoncrear.set(false);
  }
}
