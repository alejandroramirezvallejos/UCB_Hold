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
import { Componente } from '@entities/admin';
import { ComponenteService } from '@entities/component';
import { EquipoService } from '@entities/equipment';
import { Equipos } from '@entities/admin';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { AvisoExitoComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
import { CustomSelectComponent, OpcionSelect } from '@shared/ui';
@Component({
  selector: 'app-componentes-editar',
  standalone: true,
  imports: [
    FormsModule,
    MostrarerrorComponent,
    AvisoExitoComponent,
    Aviso,
    CustomSelectComponent,
  ],
  templateUrl: './componentes-editar.component.html',
  styleUrl: './componentes-editar.component.css',
})
export class ComponentesEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() componente: Componente = new Componente();
  equipos: Equipos[] = [];
  get equiposOpciones(): OpcionSelect[] {
    return this.equipos.map((equipo) => ({
      value: equipo.CodigoImt,
      label: `${equipo.NombreGrupoEquipo} ${equipo.CodigoImt}`,
    }));
  }

  constructor(
    private readonly componenteService: ComponenteService,
    private equiposAPI: EquipoService,
  ) {
    super();
  }
  ngOnInit() {
    this.cargarEquipos();
  }
  cargarEquipos() {
    this.equiposAPI.obtenerEquipos().subscribe({
      next: (data: Equipos[]) => {
        this.equipos = data;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al obtener los equipos , intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  validaredicion() {
    this.mensajeaviso = 'Estas seguro de editar este componente?';
    this.aviso.set(true);
  }
  confirmar() {
    this.componenteService.actualizarComponente(this.componente).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = 'Componente actualizado satisfactoriamente';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al actualizar el componenete , intente mas tarde',
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
