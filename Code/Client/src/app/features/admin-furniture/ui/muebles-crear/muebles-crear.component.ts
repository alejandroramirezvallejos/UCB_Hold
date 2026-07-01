import {
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Muebles } from '@entities/admin';
import { MuebleService } from '@entities/furniture';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
@Component({
  selector: 'app-muebles-crear',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent, Aviso, AvisoExitoComponent],
  templateUrl: './muebles-crear.component.html',
  styleUrl: './muebles-crear.component.css',
})
export class MueblesCrearComponent extends BaseTablaComponent {
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  mueble: Muebles = new Muebles();
  constructor(private readonly muebleapi: MuebleService) {
    super();
  }
  validarcreacion() {
    this.mensajeaviso = '¿Desea crear el mueble ' + this.mueble.Nombre + '?';
    this.aviso.set(true);
  }
  registrar() {
    this.muebleapi.crearMueble(this.mueble).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.mensajeexito =
          'Mueble ' + this.mueble.Nombre + ' creado exitosamente';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al crear el mueble , Intente mas tarde',
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
