import {
  Component,
  HostListener,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Gaveteros, Muebles } from '@entities/admin';
import { GaveteroService } from '@entities/locker';
import { MuebleService } from '@entities/furniture';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
import { CustomSelectComponent } from '@shared/ui';
@Component({
  selector: 'app-gaveteros-editar',
  imports: [
    FormsModule,
    MostrarerrorComponent,
    Aviso,
    AvisoExitoComponent,
    CustomSelectComponent,
  ],
  templateUrl: './gaveteros-editar.component.html',
  styleUrl: './gaveteros-editar.component.css',
})
export class GaveterosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() gavetero: Gaveteros = new Gaveteros();
  muebles: string[] = [];
  constructor(
    private readonly gaveteroapi: GaveteroService,
    private mueblesAPI: MuebleService,
  ) {
    super();
  }
  ngOnInit() {
    this.cargarMuebles();
  }
  cargarMuebles() {
    this.mueblesAPI.obtenerMuebles().subscribe({
      next: (data: Muebles[]) => {
        this.muebles = data.map((mueble) => mueble.Nombre ?? '');
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los muebles, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  validaredicion() {
    this.mensajeaviso = '¿Desea confirmar la edición del gavetero?';
    this.aviso.set(true);
  }
  confirmar() {
    this.gaveteroapi.editarGavetero(this.gavetero).subscribe({
      next: () => {
        this.actualizar.emit();
        this.mensajeexito = 'Gavetero editado con éxito';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al editar el gavetero, intente mas tarde',
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
