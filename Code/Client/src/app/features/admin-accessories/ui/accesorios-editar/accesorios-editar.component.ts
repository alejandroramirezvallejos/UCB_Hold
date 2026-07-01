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
import { Accesorio } from '@entities/admin';
import { AccesoriosService } from '@entities/accessory';
import { EquipoService } from '@entities/equipment';
import { Equipos } from '@entities/admin';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { extractErrorMessage } from '@shared/lib/error';
import { CustomSelectComponent, OpcionSelect } from '@shared/ui';
@Component({
  selector: 'app-accesorios-editar',
  imports: [
    FormsModule,
    MostrarerrorComponent,
    AvisoExitoComponent,
    Aviso,
    CustomSelectComponent,
  ],
  templateUrl: './accesorios-editar.component.html',
  styleUrl: './accesorios-editar.component.css',
})
export class AccesoriosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() accesorio: Accesorio = new Accesorio();
  equipos: Equipos[] = [];
  get equiposOpciones(): OpcionSelect[] {
    return this.equipos.map((e) => ({
      value: e.CodigoImt,
      label: `${e.NombreGrupoEquipo} ${e.Modelo} ${e.Marca} - ${e.CodigoImt}`,
    }));
  }

  constructor(
    private readonly accesorioapi: AccesoriosService,
    private equipoAPI: EquipoService,
  ) {
    super();
  }

  ngOnInit() {
    this.cargarEquipos();
  }

  cargarEquipos() {
    this.equipoAPI.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los equipos.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }

  confirmaredicion() {
    this.mensajeaviso = '¿Está seguro que desea editar el accesorio?';
    this.aviso.set(true);
  }

  confirmar() {
    this.accesorioapi.editarAccesorio(this.accesorio).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = 'Accesorio editado con éxito.';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al editar el accesorio.',
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
