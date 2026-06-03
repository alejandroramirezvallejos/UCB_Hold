import { Component, Input, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogApiService } from '../../../services/APIS/AuditLog/auditlog-api.service';
import { AuditLogDto } from '../../../models/admin/AuditLog';

@Component({
  selector: 'app-audit-panel',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule],
  templateUrl: './audit-panel.component.html',
  styleUrl: './audit-panel.component.css'
})
export class AuditPanelComponent implements OnInit {
  @Input() entidad!: string;

  logs: AuditLogDto[] = [];
  cargando = true;
  fechaDesde = '';
  fechaHasta = '';

  constructor(private auditService: AuditLogApiService) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.cargando = true;
    this.auditService.getAuditLog(
      this.entidad,
      undefined,
      this.fechaDesde || undefined,
      this.fechaHasta || undefined
    ).subscribe({
      next: data => { this.logs = data; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }

  badgeClass(accion: string | undefined): string {
    switch (accion?.toLowerCase()) {
      case 'crear':   return 'badge-aprobado';
      case 'editar':  return 'badge-pendiente';
      case 'aprobar': case 'recoger': return 'badge-activo';
      case 'devolver': case 'finalizado': return 'badge-finalizado';
      case 'eliminar': case 'rechazar': case 'cancelar': return 'badge-rechazado';
      case 'atrasadoautomatico': return 'badge-atrasado';
      default: return 'badge-cancelado';
    }
  }
}
