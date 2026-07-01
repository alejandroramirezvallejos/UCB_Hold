import {
  Component,
  HostListener,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
  OnChanges,
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
  selector: 'app-muebles-editar',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent, Aviso, AvisoExitoComponent],
  templateUrl: './muebles-editar.component.html',
  styleUrl: './muebles-editar.component.css',
})
export class MueblesEditarComponent
  extends BaseTablaComponent
  implements OnChanges
{
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() muebleOriginal: Muebles = new Muebles();
  mueble: Muebles = { ...this.muebleOriginal };
  constructor(private readonly muebleapi: MuebleService) {
    super();
  }
  ngOnChanges() {
    this.mueble = { ...this.muebleOriginal };
  }
  validaredicion() {
    this.mensajeaviso = '¿Desea confirmar los cambios realizados al mueble?';
    this.aviso.set(true);
  }
  confirmar() {
    this.muebleapi.actualizarMueble(this.mueble).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = 'Mueble editado exitosamente.';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al editar el mueble.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
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
