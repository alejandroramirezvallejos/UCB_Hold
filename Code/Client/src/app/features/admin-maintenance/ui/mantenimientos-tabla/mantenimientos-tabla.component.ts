import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Mantenimientos } from '@entities/admin';
import { MantenimientosCrearComponent } from '../mantenimientos-crear/mantenimientos-crear.component';
import { MantenimientoService } from '@entities/maintenance';
import { MantenimientosAgrupados } from '@entities/maintenance';
import { DetallesMantenimientoComponent } from './detalles-mantenimiento/detalles-mantenimiento.component';
import { AvisoEliminarComponent } from '@shared/ui';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { BuscadorComponent } from '@features/admin-search';
import { Tabla } from '@shared/lib/admin-table';
import { extractErrorMessage } from '@shared/lib/error';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-mantenimientos-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MantenimientosCrearComponent,
    DetallesMantenimientoComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    AuditPanelComponent,
  ],
  templateUrl: './mantenimientos-tabla.component.html',
  styleUrl: './mantenimientos-tabla.component.css',
})
export class MantenimientosTablaComponent extends Tabla implements OnInit {
  expandedRowId: number | null = null;
  auditRefresh = 0;

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }
  botoncrear: WritableSignal<boolean> = signal(false);
  mostrardetalles: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  mantenimientos: MantenimientosAgrupados[] = [];
  mantenimientoGruposeleccionado: Mantenimientos[] = [];
  mantenimientosFiltrados: MantenimientosAgrupados[] = [];
  mantenimientoSeleccionado: Mantenimientos = new Mantenimientos();
  override columnas: string[] = [
    'Empresa',
    'Códigos IMT',
    'Fecha Inicio',
    'Fecha Fin',
    'Costo',
  ];
  constructor(private readonly mantenimientoapi: MantenimientoService) {
    super();
  }
  ngOnInit() {
    this.cargarMantenimientos();
  }
  limpiarMantenimientoSeleccionado() {
    this.mantenimientoSeleccionado = new Mantenimientos();
  }
  crearmantenimiento() {
    this.botoncrear.set(true);
  }
  cargarMantenimientos() {
    this.mantenimientoapi.obtenerMantenimientos().subscribe({
      next: (datos) => {
        this.agruparMantenimientos(datos);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los mantenimientos, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  agruparMantenimientos(datos: Mantenimientos[]) {
    this.mantenimientos = [];
    if (datos.length === 0) {
      this.mantenimientosFiltrados = [];
      return;
    }
    let mantenimientosArray: Mantenimientos[] = [];
    for (let i = 0; i < datos.length; i++) {
      mantenimientosArray.push(datos[i]);
      if (i === datos.length - 1 || datos[i].Id !== datos[i + 1]?.Id) {
        this.mantenimientos.push(
          new MantenimientosAgrupados(mantenimientosArray),
        );
        mantenimientosArray = [];
      }
    }
    this.mantenimientosFiltrados = [...this.mantenimientos];
    this.aplicarOrdenActualSiExiste();
  }
  buscar() {
    this.aplicarFiltros();
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.mantenimientosFiltrados = this.mantenimientos.filter(
        (mantenimiento) => {
          switch (event[1]) {
            case 'Empresa':
              return this.normalizeText(
                mantenimiento.datosgrupo.NombreEmpresaMantenimiento || '',
              ).includes(busquedaNormalizada);
            case 'Códigos IMT':
              return this.normalizeText(
                String(mantenimiento.datosgrupo.CodigoImtEquipo || ''),
              ).includes(busquedaNormalizada);
            case 'Fecha Inicio':
              const fechaFormateada = this.formatDate(
                mantenimiento.datosgrupo.FechaMantenimiento,
              );
              return this.normalizeText(fechaFormateada).includes(
                busquedaNormalizada,
              );
            case 'Fecha Fin':
              return this.normalizeText(
                this.formatDate(
                  mantenimiento.datosgrupo.FechaFinalDeMantenimiento,
                ),
              ).includes(busquedaNormalizada);
            case 'Costo':
              return this.normalizeText(
                String(mantenimiento.datosgrupo.Costo || ''),
              ).includes(busquedaNormalizada);
            default:
              return (
                this.normalizeText(
                  mantenimiento.datosgrupo.NombreEmpresaMantenimiento || '',
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  mantenimiento.datosgrupo.TipoMantenimiento || '',
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  mantenimiento.datosgrupo.NombreGrupoEquipo || '',
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  String(mantenimiento.datosgrupo.CodigoImtEquipo || ''),
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  this.formatDate(mantenimiento.datosgrupo.FechaMantenimiento),
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  this.formatDate(
                    mantenimiento.datosgrupo.FechaFinalDeMantenimiento,
                  ),
                ).includes(busquedaNormalizada) ||
                this.normalizeText(
                  String(mantenimiento.datosgrupo.Costo || ''),
                ).includes(busquedaNormalizada)
              );
          }
        },
      );
    } else {
      this.mantenimientosFiltrados = [...this.mantenimientos];
    }
    this.aplicarOrdenActualSiExiste();
  }
  limpiarBusqueda() {
    this.aplicarFiltros();
  }
  eliminarMantenimiento(mantenimiento: MantenimientosAgrupados) {
    this.mantenimientoSeleccionado = mantenimiento.matenimientos[0];
    this.alertaeliminar = true;
  }
  confirmarEliminacion() {
    this.mantenimientoapi
      .eliminarMantenimiento(this.mantenimientoSeleccionado.Id)
      .subscribe({
        next: () => {
          this.limpiarMantenimientoSeleccionado();
          this.alertaeliminar = false;
          this.mensajeexito = 'Mantenimiento eliminado exitosamente';
          this.exito.set(true);
          this.auditRefresh++;
          this.cargarMantenimientos();
        },
        error: (error) => {
          const errorMsg = extractErrorMessage(
            error,
            'Error al eliminar el mantenimiento, intente mas tarde',
          );
          this.mensajeerror = errorMsg;
          this.error.set(true);
          this.limpiarMantenimientoSeleccionado();
          this.alertaeliminar = false;
        },
      });
  }
  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarMantenimientoSeleccionado();
  }
  mostrarmantenimientosindividuales(
    mantenimientosgrupo: MantenimientosAgrupados,
  ) {
    this.mantenimientoGruposeleccionado = mantenimientosgrupo.matenimientos;
    this.mostrardetalles.set(true);
  }

  override sortTable(e: { col: string; dir: 'asc' | 'desc' }): void {
    this.mantenimientosFiltrados = this.sortByColumn(
      this.mantenimientosFiltrados,
      e,
      {
        Empresa: (mantenimiento) =>
          mantenimiento.datosgrupo.NombreEmpresaMantenimiento,
        'Códigos IMT': (mantenimiento) =>
          mantenimiento.datosgrupo.CodigoImtEquipo,
        'Fecha Inicio': (mantenimiento) =>
          mantenimiento.datosgrupo.FechaMantenimiento,
        'Fecha Fin': (mantenimiento) =>
          mantenimiento.datosgrupo.FechaFinalDeMantenimiento,
        Costo: (mantenimiento) => mantenimiento.datosgrupo.Costo,
        'Tipo y nombre equipos': (mantenimiento) =>
          `${mantenimiento.datosgrupo.TipoMantenimiento ?? ''} ${mantenimiento.datosgrupo.NombreGrupoEquipo ?? ''}`,
      },
    );
  }
}
