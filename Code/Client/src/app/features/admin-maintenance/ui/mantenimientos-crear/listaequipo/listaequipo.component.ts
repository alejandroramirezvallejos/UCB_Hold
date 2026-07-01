import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Equipos } from '@entities/admin';
import { EquipoService } from '@entities/equipment';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MantenimientosServiceEquipos } from '../../../model/mantenimientos-equipos.service';
import { FormularioDatosComponent } from './formulario-datos/formulario-datos.component';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';

type EquipoOrdenColumna =
  'Nombre' | 'EstadoEquipo' | 'Ubicacion' | 'CodigoImt' | 'CostoReferencia';

const EQUIPO_ORDEN_CAMPO: Record<EquipoOrdenColumna, keyof Equipos> = {
  Nombre: 'NombreGrupoEquipo',
  EstadoEquipo: 'EstadoEquipo',
  Ubicacion: 'Ubicacion',
  CodigoImt: 'CodigoImt',
  CostoReferencia: 'CostoReferencia',
};

@Component({
  selector: 'app-listaequipo',
  imports: [
    CommonModule,
    FormsModule,
    FormularioDatosComponent,
    MostrarerrorComponent,
  ],
  templateUrl: './listaequipo.component.html',
  styleUrl: './listaequipo.component.css',
})
export class ListaequipoComponent extends BaseTablaComponent {
  @Input() agregarequipo: WritableSignal<boolean> = signal(true);
  equipos: Equipos[] = [];
  equiposcopia: Equipos[] = [];
  equipoSeleccionado: Equipos = new Equipos();
  terminoBusqueda: string = '';
  agregarEquipoSeleccionado: WritableSignal<boolean> = signal(false);
  sortColumn: EquipoOrdenColumna = 'Nombre';
  sortDirection: 'asc' | 'desc' = 'asc';

  get equiposDisponibles(): Equipos[] {
    return this.equipos.filter((equipo) => !this.equipoYaSeleccionado(equipo));
  }

  constructor(
    private readonly equiposapi: EquipoService,
    public mantenimientoequipos: MantenimientosServiceEquipos,
  ) {
    super();
  }
  ngOnInit() {
    this.cargarEquipos();
  }
  agregarEquipo(equipo: Equipos) {
    this.equipoSeleccionado = equipo;
    this.agregarEquipoSeleccionado.set(true);
  }

  equipoYaSeleccionado(equipo: Equipos): boolean {
    return (
      equipo.CodigoImt !== null &&
      this.mantenimientoequipos.existeEquipo(equipo.CodigoImt)
    );
  }

  limpiarEquipoSeleccionado() {
    this.equipoSeleccionado = new Equipos();
  }
  cargarEquipos() {
    this.equiposapi.obtenerEquipos().subscribe({
      next: (data: Equipos[]) => {
        this.equipos = data;
        this.equiposcopia = [...this.equipos];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los equipos, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  private normalizeText(text: unknown): string {
    return String(text ?? '')
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');
  }
  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }
    const busquedaNormalizada = this.normalizeText(this.terminoBusqueda);
    this.equipos = this.equiposcopia.filter(
      (equipo) =>
        this.normalizeText(equipo.NombreGrupoEquipo).includes(
          busquedaNormalizada,
        ) ||
        this.normalizeText(equipo.Modelo).includes(busquedaNormalizada) ||
        this.normalizeText(equipo.Marca).includes(busquedaNormalizada) ||
        this.normalizeText(String(equipo.CodigoImt || '')).includes(
          busquedaNormalizada,
        ) ||
        this.normalizeText(equipo.CodigoUcb).includes(busquedaNormalizada) ||
        this.normalizeText(equipo.NumeroSerial).includes(busquedaNormalizada) ||
        this.normalizeText(equipo.NombreGrupoEquipo).includes(
          busquedaNormalizada,
        ),
    );
  }
  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.equipos = [...this.equiposcopia];
  }
  aplicarOrdenamiento() {
    const campo = EQUIPO_ORDEN_CAMPO[this.sortColumn];

    this.equipos.sort((a, b) => {
      const compA = this.normalizeText(a[campo]);
      const compB = this.normalizeText(b[campo]);

      if (compA < compB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }

      if (compA > compB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }

      return 0;
    });
  }
  ordenarPor(columna: EquipoOrdenColumna) {
    if (this.sortColumn === columna) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columna;
      this.sortDirection = 'asc';
    }
    this.aplicarOrdenamiento();
  }
  regresar() {
    this.agregarequipo.set(false);
  }
}
