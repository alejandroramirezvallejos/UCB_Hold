import { Component, HostListener, EventEmitter, Input, Output, signal, WritableSignal, OnChanges } from '@angular/core';
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
  selector: 'app-grupos-equipos-editar',
  standalone: true,
  imports: [FormsModule , MostrarerrorComponent, Aviso , AvisoExitoComponent, CustomSelectComponent],
  templateUrl: './grupos-equipos-editar.component.html',
  styleUrl: './grupos-equipos-editar.component.css'
})
export class GruposEquiposEditarComponent extends BaseTablaComponent implements OnChanges {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() categorias: string[] = [];
  @Input() grupoequipo: GrupoEquipo = new GrupoEquipo();
  grupoEquipo: GrupoEquipo = { ...this.grupoequipo };
  constructor(private readonly grupoEquipoapi: GrupoequipoService) {
    super();
   }
  ngOnChanges() {
    this.grupoEquipo = { ...this.grupoequipo };
  }
  validaredicion(){
    this.mensajeaviso="¿Desea guardar los cambios realizados al grupo de equipo?";
    this.aviso.set(true);
  }
  confirmar() {
    this.grupoEquipoapi.editarGrupoEquipo(this.grupoEquipo).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito="Grupo de equipo editado exitosamente";
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al editar el grupo de equipo");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar() {
    this.botoneditar.set(false);
  }
  @HostListener('click', ['$event'])
  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) this.cerrar();
  }

}