import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogApiService } from '../../../services/APIS/AuditLog/auditlog-api.service';
import { AuditLogDto } from '../../../models/admin/AuditLog';

const ACCIONES_POR_ENTIDAD: Record<string, string[]> = {
  Prestamo:  ['Crear', 'Editar', 'Aprobar', 'Rechazar', 'Recoger', 'Devolver', 'Cancelar'],
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
  imports: [CommonModule, DatePipe, FormsModule],
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
  today = new Date().toISOString().split('T')[0];

  get acciones(): string[] {
    return ACCIONES_POR_ENTIDAD[this.entidad] ?? ['Crear', 'Editar', 'Eliminar'];
  }

  constructor(private auditService: AuditLogApiService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['entidad'] || changes['refreshTrigger'])
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

  limpiar() {
    this.fechaDesde = '';
    this.fechaHasta = '';
    this.filtroAccion = '';
    this.filtroAdmin = '';
    this.cargar();
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
