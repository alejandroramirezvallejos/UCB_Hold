import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Carrera } from '@entities/admin';
import { CarrerasCrearComponent } from '../carreras-crear/carreras-crear.component';
import { CarrerasEditarComponent } from '../carreras-editar/carreras-editar.component';
import { CarreraService } from '@entities/career';
import { AvisoEliminarComponent } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { BuscadorComponent } from '@features/admin-search';
import { Tabla } from '@shared/lib/admin-table';
import { extractErrorMessage } from '@shared/lib/error';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-carreras-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    CarrerasCrearComponent,
    CarrerasEditarComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    AuditPanelComponent,
  ],
  templateUrl: './carreras-tabla.component.html',
  styleUrl: './carreras-tabla.component.css',
})
export class CarrerasTablaComponent extends Tabla {
  expandedRowId: number | null = null;
  auditRefresh = 0;

  toggleExpand(id: number): void {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  carreras: Carrera[] = [];
  carrerascopia: Carrera[] = [];
  carreraSeleccionada: Carrera = new Carrera();

  constructor(private readonly carreraService: CarreraService) {
    super();
  }

  ngOnInit(): void {
    this.cargarCarreras();
  }

  limpiarCarreraSeleccionada(): void {
    this.carreraSeleccionada = new Carrera();
  }

  crearCarrera(): void {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }

  cargarCarreras(): void {
    this.carreraService.obtenerCarreras().subscribe({
      next: (data: Carrera[]) => {
        this.carreras = data;
        this.carrerascopia = [...this.carreras];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar las carreras , intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }

  aplicarFiltros(event?: [string, string]): void {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.carreras = this.carrerascopia.filter((carrera) => {
        switch (event[1]) {
          case 'Nombre':
            return this.normalizeText(carrera.Nombre || '').includes(
              busquedaNormalizada,
            );
          default:
            return this.normalizeText(carrera.Nombre || '').includes(
              busquedaNormalizada,
            );
        }
      });
    } else {
      this.carreras = [...this.carrerascopia];
    }
  }

  limpiarBusqueda(): void {
    this.carreras = [...this.carrerascopia];
  }

  editarCarrera(carrera: Carrera): void {
    this.botoncrear.set(false);
    this.carreraSeleccionada = { ...carrera };
    this.botoneditar.set(true);
  }

  eliminarCarrera(carrera: Carrera): void {
    this.carreraSeleccionada = carrera;
    this.alertaeliminar = true;
  }

  confirmarEliminacion(): void {
    if (this.carreraSeleccionada.Id) {
      this.carreraService
        .eliminarCarrera(this.carreraSeleccionada.Id)
        .subscribe({
          next: () => {
            this.cargarCarreras();
            this.mensajeexito = 'Carrera eliminada correctamente';
            this.exito.set(true);
            this.auditRefresh++;
          },
          error: (error) => {
            const errorMsg = extractErrorMessage(
              error,
              'Error al eliminar la carrera',
            );
            this.mensajeerror = errorMsg;
            this.error.set(true);
          },
        });
    }
    this.limpiarCarreraSeleccionada();
    this.alertaeliminar = false;
  }

  cancelarEliminacion(): void {
    this.alertaeliminar = false;
    this.limpiarCarreraSeleccionada();
  }
}
