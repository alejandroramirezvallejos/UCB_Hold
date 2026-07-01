import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GrupoEquipo } from '@entities/equipment-group';
import { GrupoequipoService } from '@entities/equipment-group';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
import { CustomSelectComponent } from '@shared/ui';
@Component({
  selector: 'app-grupos-equipos-crear',
  standalone: true,
  imports: [
    FormsModule,
    MostrarerrorComponent,
    Aviso,
    AvisoExitoComponent,
    CustomSelectComponent,
  ],
  templateUrl: './grupos-equipos-crear.component.html',
  styleUrl: './grupos-equipos-crear.component.css',
})
export class GruposEquiposCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Input() categorias: string[] = [];
  @Output() Actualizar = new EventEmitter<void>();
  grupoEquipo: GrupoEquipo = new GrupoEquipo();
  constructor(private readonly grupoEquipoapi: GrupoequipoService) {
    super();
  }
  validarregistro() {
    this.mensajeaviso = 'Desea registrar el nuevo grupo de equipo?';
    this.aviso.set(true);
  }
  registrar() {
    this.grupoEquipoapi.crearGrupoEquipo(this.grupoEquipo).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito = 'Grupo de equipo registrado exitosamente';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al registrar el grupo de equipo',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  cerrar() {
    this.botoncrear.set(false);
  }
}
