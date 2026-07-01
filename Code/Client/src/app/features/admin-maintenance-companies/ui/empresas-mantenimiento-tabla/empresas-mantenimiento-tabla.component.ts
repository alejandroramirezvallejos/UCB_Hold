import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '@entities/admin';
import { EmpresasMantenimientoCrearComponent } from '../empresas-mantenimiento-crear/empresas-mantenimiento-crear.component';
import { EmpresasMantenimientoEditarComponent } from '../empresas-mantenimiento-editar/empresas-mantenimiento-editar.component';
import { EmpresamantenimientoService } from '@entities/maintenance-company';
import { AvisoEliminarComponent } from '@shared/ui';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { Tabla } from '@shared/lib/admin-table';
import { BuscadorComponent } from '@features/admin-search';
import { extractErrorMessage } from '@shared/lib/error';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-empresas-mantenimiento-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    EmpresasMantenimientoCrearComponent,
    EmpresasMantenimientoEditarComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    AuditPanelComponent,
  ],
  templateUrl: './empresas-mantenimiento-tabla.component.html',
  styleUrl: './empresas-mantenimiento-tabla.component.css',
})
export class EmpresasMantenimientoTablaComponent
  extends Tabla
  implements OnInit
{
  expandedRowId: number | null = null;
  auditRefresh = 0;

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }
  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  empresas: EmpresaMantenimiento[] = [];
  empresascopia: EmpresaMantenimiento[] = [];
  empresaSeleccionada: EmpresaMantenimiento = new EmpresaMantenimiento();
  override columnas: string[] = [
    'Nombre Empresa',
    'Responsable',
    'Teléfono',
    'NIT',
  ];
  constructor(private readonly empresaService: EmpresamantenimientoService) {
    super();
  }
  ngOnInit() {
    this.cargarEmpresas();
  }
  limpiarEmpresaSeleccionada() {
    this.empresaSeleccionada = new EmpresaMantenimiento();
  }
  crearempresamantenimiento() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarEmpresas() {
    this.empresaService.obtenerEmpresaMantenimiento().subscribe({
      next: (data: EmpresaMantenimiento[]) => {
        this.empresas = data;
        this.empresascopia = [...this.empresas];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar las empresas de mantenimiento, intente más tarde.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.empresas = this.empresascopia.filter((empresa) => {
        switch (event[1]) {
          case 'Nombre Empresa':
            return this.normalizeText(empresa.NombreEmpresa || '').includes(
              busquedaNormalizada,
            );
          case 'Responsable':
            return this.normalizeText(
              empresa.NombreResponsable ||
                '' + empresa.ApellidoResponsable ||
                '',
            ).includes(busquedaNormalizada);
          case 'Teléfono':
            return this.normalizeText(empresa.Telefono || '').includes(
              busquedaNormalizada,
            );
          case 'NIT':
            return this.normalizeText(empresa.Nit || '').includes(
              busquedaNormalizada,
            );
          default:
            return (
              this.normalizeText(empresa.NombreEmpresa || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(empresa.NombreResponsable || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(empresa.Telefono || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(empresa.Nit || '').includes(
                busquedaNormalizada,
              )
            );
        }
      });
    } else {
      this.empresas = [...this.empresascopia];
    }
  }
  limpiarBusqueda() {
    this.empresas = [...this.empresascopia];
  }
  editarEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    this.botoncrear.set(false);
    this.empresaSeleccionada = { ...empresa };
    this.botoneditar.set(true);
  }
  eliminarEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    this.empresaSeleccionada = empresa;
    this.alertaeliminar = true;
  }
  confirmarEliminacion() {
    if (this.empresaSeleccionada.Id) {
      this.empresaService
        .eliminarEmpresaMantenimiento(this.empresaSeleccionada.Id)
        .subscribe({
          next: (response) => {
            this.cargarEmpresas();
            this.mensajeexito =
              'Empresa de mantenimiento eliminada exitosamente.';
            this.exito.set(true);
            this.auditRefresh++;
          },
          error: (error) => {
            const errorMsg = extractErrorMessage(
              error,
              'Error al eliminar la empresa de mantenimiento.',
            );
            this.mensajeerror = errorMsg;
            this.error.set(true);
          },
        });
    }
    this.limpiarEmpresaSeleccionada();
    this.alertaeliminar = false;
  }
  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarEmpresaSeleccionada();
  }
}
