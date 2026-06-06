import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { HistorialEquipoDto } from '../../../models/admin/HistorialEquipo';

@Component({
  selector: 'app-historial-equipo-inline',
  standalone: true,
  imports: [CommonModule, DatePipe],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando historial...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin préstamos registrados para este equipo</p> }
      @else {
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Usuario</th>
              <th>Carnet</th>
              <th>Fecha Préstamo</th>
              <th>Fecha Devolución</th>
              <th>Estado</th>
              <th>Estado Equipo</th>
              <th>Observación</th>
            </tr>
          </thead>
          <tbody>
            @for(h of items; track h.IdPrestamo) {
              <tr>
                <td>#{{ h.IdPrestamo }}</td>
                <td>{{ h.NombreUsuario }}</td>
                <td>{{ h.Carnet }}</td>
                <td>{{ h.FechaPrestamo | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                <td>{{ (h.FechaDevolucion || h.FechaDevolucionEsperada) | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                <td><span [class]="'badge badge-' + (h.EstadoPrestamo || 'cancelado')">{{ h.EstadoPrestamo }}</span></td>
                <td><span [class]="'badge badge-estado-' + (h.EstadoEquipo || 'none')">{{ estadoEquipoLabel(h.EstadoEquipo) }}</span></td>
                @if(h.Observacion) {
                  <td class="obs-cell" (click)="observacionClick.emit(h.Observacion)" title="Ver observación completa">{{ h.Observacion }}</td>
                } @else {
                  <td>—</td>
                }
              </tr>
            }
          </tbody>
        </table>
      }
    </div>
  `,
  styles: [`
    .badge-estado-operativo { background: var(--success-bg); color: var(--success); }
    .badge-estado-parcialmente_operativo { background: #fff7ed; color: #b45309; }
    .badge-estado-inoperativo { background: var(--error-bg); color: var(--error); }
    .badge-estado-none { background: var(--sidebar); color: var(--ink-muted); }
    .obs-cell { max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; cursor: pointer; color: var(--interactive-text); }
    .obs-cell:hover { text-decoration: underline; }
  `]
})
export class HistorialEquipoInlineComponent implements OnInit {
  @Input() equipoId!: number;
  @Output() observacionClick = new EventEmitter<string>();
  items: HistorialEquipoDto[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

  estadoEquipoLabel(estado?: string): string {
    switch (estado) {
      case 'operativo': return 'Operativo';
      case 'parcialmente_operativo': return 'Parcialmente operativo';
      case 'inoperativo': return 'Inoperativo';
      default: return '—';
    }
  }

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Equipo/${this.equipoId}/historial`).subscribe({
      next: res => {
        const data = res?.Value ?? res?.value ?? [];
        this.items = (Array.isArray(data) ? data : []).map((item: any) => ({
          ...item,
          FechaPrestamo: item.FechaPrestamo ? new Date(item.FechaPrestamo) : null,
          FechaDevolucionEsperada: item.FechaDevolucionEsperada ? new Date(item.FechaDevolucionEsperada) : null,
          FechaDevolucion: item.FechaDevolucion ? new Date(item.FechaDevolucion) : null,
        }));
        this.cargando = false;
      },
      error: () => { this.cargando = false; }
    });
  }
}
