import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogApiService } from '../../../services/APIS/AuditLog/auditlog-api.service';
import { AuditLogDto } from '../../../models/admin/AuditLog';
import { FlatpickrDirective } from '../../../directives/flatpickr.directive';

const ACCIONES_POR_ENTIDAD: Record<string, string[]> = {
  Prestamo:  ['Aprobar', 'Rechazar', 'Recoger', 'Devolver', 'Cancelar'],
  Usuario:   ['Crear', 'Editar', 'Eliminar'],
  Equipo:    ['Crear', 'Editar', 'Eliminar'],
  GrupoEquipo:          ['Crear', 'Editar', 'Eliminar'],
  Accesorio:            ['Crear', 'Editar', 'Eliminar'],
  Componente:           ['Crear', 'Editar', 'Eliminar'],
  Gavetero:             ['Crear', 'Editar', 'Eliminar'],
  Mueble:               ['Crear', 'Editar', 'Eliminar'],
  Mantenimiento:        ['Crear', 'Editar', 'Eliminar'],
  EmpresaMantenimiento: ['Crear', 'Editar', 'Eliminar'],
  Carrera:              ['Crear', 'Editar', 'Eliminar'],
  Categoria:            ['Crear', 'Editar', 'Eliminar'],
};

@Component({
  selector: 'app-audit-panel',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, FlatpickrDirective],
  templateUrl: './audit-panel.component.html',
  styleUrl: './audit-panel.component.css'
})
export class AuditPanelComponent implements OnChanges {
  @Input() entidad!: string;
  @Input() refreshTrigger: number = 0;

  logs: AuditLogDto[] = [];
  cargando = true;
  fechaDesde = '';
  fechaHasta = '';
  filtroAccion = '';
  filtroAdmin = '';

  get acciones(): string[] {
    return ACCIONES_POR_ENTIDAD[this.entidad] ?? ['Crear', 'Editar', 'Eliminar'];
  }

  constructor(private readonly auditService: AuditLogApiService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['entidad'] || changes['refreshTrigger'])
      this.cargar();
  }

  onFechaDesde(dates: Date[]) {
    this.fechaDesde = dates[0] ? dates[0].toISOString().split('T')[0] : '';
    this.cargar();
  }

  onFechaHasta(dates: Date[]) {
    this.fechaHasta = dates[0] ? dates[0].toISOString().split('T')[0] : '';
    this.cargar();
  }

  cargar() {
    this.cargando = true;
    this.auditService.getAuditLog(
      this.entidad,
      this.filtroAdmin || undefined,
      this.fechaDesde || undefined,
      this.fechaHasta || undefined
    ).subscribe({
      next: data => {
        this.logs = this.filtroAccion
          ? data.filter(l => l.Accion?.toLowerCase() === this.filtroAccion.toLowerCase())
          : data;
        this.cargando = false;
      },
      error: () => { this.cargando = false; }
    });
  }

  accionesOpen = false;

  toggleAcciones() { this.accionesOpen = !this.accionesOpen; }

  seleccionarAccion(a: string) {
    this.filtroAccion = a;
    this.accionesOpen = false;
    this.cargar();
  }

  limpiarFiltroAdmin() {
    this.filtroAdmin = '';
    this.cargar();
  }

  limpiar() {
    this.fechaDesde = '';
    this.fechaHasta = '';
    this.filtroAccion = '';
    this.filtroAdmin = '';
    this.accionesOpen = false;
    this.cargar();
  }

  // === Observación: parseo + modal de detalle ===
  obsAbierta: { observacion?: string; equipos?: { codigo: number; nombre?: string; estado: string }[]; texto?: string } | null = null;

  parseDetalle(detalle?: string): { observacion?: string; equipos?: { codigo: number; nombre?: string; estado: string }[]; texto?: string } | null {
    if (!detalle) return null;
    try {
      const o = JSON.parse(detalle);
      if (o && typeof o === 'object') return o;
    } catch { /* texto plano */ }
    return { texto: detalle };
  }

  resumenObs(log: AuditLogDto): string {
    const p = this.parseDetalle(log.Detalle);
    if (!p) return '—';
    return p.observacion || p.texto || (p.equipos?.length ? 'Ver estados de equipos' : '—');
  }

  tieneDetalle(log: AuditLogDto): boolean {
    const p = this.parseDetalle(log.Detalle);
    return !!p && !!(p.observacion || p.texto || p.equipos?.length);
  }

  abrirObs(log: AuditLogDto): void {
    const p = this.parseDetalle(log.Detalle);
    if (p) this.obsAbierta = p;
  }

  cerrarObs(): void { this.obsAbierta = null; }

  estadoEquipoLabel(estado?: string): string {
    switch (estado) {
      case 'operativo': return 'Operativo';
      case 'parcialmente_operativo': return 'Parcialmente operativo';
      case 'inoperativo': return 'Inoperativo';
      default: return estado || '—';
    }
  }

  badgeClass(accion: string | undefined): string {
    switch (accion?.toLowerCase()) {
      case 'crear':   return 'badge-aprobado';
      case 'editar':  return 'badge-pendiente';
      case 'aprobar': case 'recoger': return 'badge-activo';
      case 'devolver': return 'badge-finalizado';
      case 'eliminar': case 'rechazar': case 'cancelar': return 'badge-rechazado';
      case 'atrasadoautomatico': return 'badge-atrasado';
      default: return 'badge-cancelado';
    }
  }
}
