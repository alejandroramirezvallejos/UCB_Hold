import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Gaveteros } from '@entities/admin';
import { GaveterosCrearComponent } from '../gaveteros-crear/gaveteros-crear.component';
import { GaveterosEditarComponent } from '../gaveteros-editar/gaveteros-editar.component';
import { GaveteroService } from '@entities/locker';
import { AvisoEliminarComponent } from '@shared/ui';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { BuscadorComponent } from '@features/admin-search';
import { Tabla } from '@shared/lib/admin-table';
import { extractErrorMessage } from '@shared/lib/error';
import { EquiposGaveteroInlineComponent } from '@widgets/admin-inline';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-gaveteros-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    GaveterosCrearComponent,
    GaveterosEditarComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    EquiposGaveteroInlineComponent,
    AuditPanelComponent,
  ],
  templateUrl: './gaveteros-tabla.component.html',
  styleUrls: ['./gaveteros-tabla.component.css'],
})
export class GaveterosTablaComponent extends Tabla {
  expandedRowId: number | null = null;
  auditRefresh = 0;

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  gaveteros: Gaveteros[] = [];
  gaveteroscopia: Gaveteros[] = [];
  gaveteroSeleccionado: Gaveteros = new Gaveteros();
  override columnas: string[] = [
    'Nombre',
    'Tipo',
    'Nombre Mueble',
    'Longitud',
    'Altura',
    'Profundidad',
  ];
  constructor(private readonly gaveterosapi: GaveteroService) {
    super();
  }
  ngOnInit() {
    this.cargarGaveteros();
  }
  limpiarGaveteroSeleccionado() {
    this.gaveteroSeleccionado = new Gaveteros();
  }
  creargarvetero() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarGaveteros() {
    this.gaveterosapi.obtenerGaveteros().subscribe({
      next: (data: Gaveteros[]) => {
        this.gaveteros = data;
        this.gaveteroscopia = [...this.gaveteros];
        this.aplicarOrdenActualSiExiste();
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los gaveteros, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.gaveteros = this.gaveteroscopia.filter((gavetero) => {
        switch (event[1]) {
          case 'Nombre':
            return this.normalizeText(gavetero.Nombre || '').includes(
              busquedaNormalizada,
            );
          case 'Tipo':
            return this.normalizeText(gavetero.Tipo || '').includes(
              busquedaNormalizada,
            );
          case 'Nombre Mueble':
            return this.normalizeText(gavetero.NombreMueble || '').includes(
              busquedaNormalizada,
            );
          case 'Longitud':
            return this.normalizeText(String(gavetero.Longitud || '')).includes(
              busquedaNormalizada,
            );
          case 'Altura':
            return this.normalizeText(String(gavetero.Altura || '')).includes(
              busquedaNormalizada,
            );
          case 'Profundidad':
            return this.normalizeText(
              String(gavetero.Profundidad || ''),
            ).includes(busquedaNormalizada);
          default:
            return (
              this.normalizeText(gavetero.Nombre || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(gavetero.Tipo || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(gavetero.NombreMueble || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(String(gavetero.Longitud || '')).includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(String(gavetero.Altura || '')).includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(String(gavetero.Profundidad || '')).includes(
                busquedaNormalizada,
              )
            );
        }
      });
    } else {
      this.gaveteros = [...this.gaveteroscopia];
    }
    this.aplicarOrdenActualSiExiste();
  }
  limpiarBusqueda() {
    this.gaveteros = [...this.gaveteroscopia];
    this.aplicarOrdenActualSiExiste();
  }
  editarGavetero(gavetero: Gaveteros) {
    this.botoncrear.set(false);
    this.gaveteroSeleccionado = { ...gavetero };
    this.botoneditar.set(true);
  }
  eliminarGavetero(gavetero: Gaveteros) {
    this.gaveteroSeleccionado = gavetero;
    this.alertaeliminar = true;
  }
  confirmarEliminacion() {
    this.gaveterosapi.eliminarGavetero(this.gaveteroSeleccionado.Id).subscribe({
      next: (response) => {
        this.mensajeexito = 'Gavetero eliminado con exito';
        this.exito.set(true);
        this.auditRefresh++;
        this.cargarGaveteros();
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al eliminar el gavetero, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
    this.limpiarGaveteroSeleccionado();
    this.alertaeliminar = false;
  }
  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarGaveteroSeleccionado();
  }

  override sortTable(e: { col: string; dir: 'asc' | 'desc' }) {
    const m: Record<string, keyof Gaveteros> = {
      Nombre: 'Nombre',
      Tipo: 'Tipo',
      'Nombre Mueble': 'NombreMueble',
      Longitud: 'Longitud',
      Altura: 'Altura',
      Profundidad: 'Profundidad',
    };
    const k = m[e.col];
    if (!k) return;
    this.gaveteros = [...this.gaveteros].sort((a, b) =>
      this.compareSortableValues(a[k], b[k], e.dir),
    );
  }
}
