import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { HistorialEquipoDto } from '../../../models/admin/HistorialEquipo';
import { FlatpickrDirective } from '../../../directives/flatpickr.directive';
import flatpickr from 'flatpickr';

const ESTADOS_PRESTAMO = ['pendiente', 'aprobado', 'activo', 'atrasado', 'finalizado', 'rechazado', 'cancelado'];

@Component({
  selector: 'app-historial-equipo-inline',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, FlatpickrDirective],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando historial...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin préstamos registrados para este equipo</p> }
      @else {
        <div class="audit-filters">
          <div class="audit-search-wrap">
            <i class="fas fa-search search-icon"></i>
            <input type="text" [(ngModel)]="filtroTexto" placeholder="Buscar por # préstamo o usuario..." class="admin-search">
            @if (filtroTexto) {
              <button class="clear-search" (click)="filtroTexto = ''" title="Limpiar búsqueda" type="button">
                <i class="fas fa-times"></i>
              </button>
            }
          </div>
          <input
            type="text"
            appFlatpickr
            [fpOptions]="{maxDate: 'today'}"
            class="date-input"
            placeholder="Fecha desde"
            readonly
            (fpChange)="onFechaDesde($event)"
            (fpReady)="fpDesde = $event"
          />
          <input
            type="text"
            appFlatpickr
            [fpOptions]="{maxDate: 'today'}"
            class="date-input"
            placeholder="Fecha hasta"
            readonly
            (fpChange)="onFechaHasta($event)"
            (fpReady)="fpHasta = $event"
          />
          <select class="date-input" [(ngModel)]="filtroEstado">
            <option value="">Todos los estados</option>
            @for(e of estadosPrestamo; track e) {
              <option [value]="e">{{ estadoEquipoLabel(e) }}</option>
            }
          </select>
          <div class="audit-filters-actions">
            <button class="btn btn-ghost btn-sm" (click)="limpiarFiltros()">
              <i class="fas fa-times"></i> Limpiar
            </button>
          </div>
        </div>
        @if(itemsFiltrados.length === 0) {
          <p class="empty-inline">Ningún préstamo coincide con los filtros</p>
        } @else {
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
              @for(h of itemsFiltrados; track h.IdPrestamo) {
                <tr>
                  <td>#{{ h.IdPrestamo }}</td>
                  <td>{{ h.NombreUsuario }}</td>
                  <td>{{ h.Carnet }}</td>
                  <td>{{ h.FechaPrestamo | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                  <td>{{ (h.FechaDevolucion || h.FechaDevolucionEsperada) | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                  <td><span [class]="'badge badge-' + (h.EstadoPrestamo || 'cancelado')">{{ h.EstadoPrestamo }}</span></td>
                  <td><span [class]="'badge badge-estado-' + (h.EstadoEquipo || 'none')">{{ estadoEquipoBadgeLabel(h.EstadoEquipo) }}</span></td>
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

    /* === Filtros estilo auditoría === */
    .audit-filters {
      display: flex;
      gap: 0.625rem;
      align-items: center;
      margin-bottom: 1rem;
      flex-wrap: wrap;
    }
    .audit-search-wrap {
      position: relative;
      flex: 1 1 200px;
      min-width: 180px;
      max-width: 22rem;
    }
    .audit-search-wrap .search-icon {
      position: absolute;
      left: 1rem;
      top: 50%;
      transform: translateY(-50%);
      color: var(--ink-muted);
      font-size: 0.85rem;
      pointer-events: none;
    }
    .admin-search {
      width: 100%;
      height: 42px;
      padding: 0 2.25rem 0 2.5rem;
      border: 1px solid var(--border);
      border-radius: var(--radius-full);
      font-size: 0.875rem;
      font-weight: 500;
      font-family: var(--font);
      color: var(--ink);
      background: rgba(255, 255, 255, 0.9);
      box-sizing: border-box;
      transition:
        border-color var(--t-fast),
        box-shadow var(--t-fast);
    }
    .admin-search::placeholder {
      color: #757575;
      font-weight: 500;
    }
    .admin-search:focus {
      outline: none;
      border-color: var(--interactive-text);
      box-shadow: 0 0 0 3px rgba(0, 69, 123, 0.12);
    }
    .clear-search {
      position: absolute;
      right: 0.6rem;
      top: 50%;
      transform: translateY(-50%);
      display: flex;
      align-items: center;
      justify-content: center;
      width: 1.4rem;
      height: 1.4rem;
      border: none;
      background: none;
      cursor: pointer;
      padding: 0;
      border-radius: 50%;
      color: var(--ink-muted);
      font-size: 0.75rem;
      transition:
        color var(--t-fast),
        background-color var(--t-fast),
        transform var(--t-fast);
    }
    @media (hover: hover) and (pointer: fine) {
      .clear-search:hover {
        color: var(--ink);
        background: var(--interactive-subtle);
      }
    }
    .clear-search:active {
      transform: translateY(-50%) scale(0.92);
    }
    .date-input {
      padding: 0.5rem 1.25rem;
      border: 1px solid var(--border);
      border-radius: var(--radius-full);
      font-size: 0.875rem;
      font-family: var(--font);
      color: var(--ink);
      background: rgba(255, 255, 255, 0.9);
      height: 42px;
      box-sizing: border-box;
      transition:
        border-color var(--t-fast),
        box-shadow var(--t-fast);
    }
    .date-input:focus {
      outline: none;
      border-color: var(--interactive-text);
      box-shadow: 0 0 0 3px rgba(0, 69, 123, 0.12);
    }
    .date-input::placeholder {
      color: #757575;
    }
    .audit-filters-actions {
      display: flex;
      align-items: center;
      gap: 0.625rem;
    }

    @media (max-width: 768px) {
      .audit-filters {
        flex-direction: column;
        align-items: stretch;
      }
      .audit-search-wrap,
      .date-input,
      .admin-search {
        width: 100%;
        max-width: none;
        min-width: 0;
        flex: 0 0 auto;
      }
      .date-input {
        height: 40px;
        padding: 0.4rem 1.1rem;
        font-size: 16px;
      }
      .admin-search {
        height: 40px;
        font-size: 16px;
      }
      .audit-filters-actions {
        width: 100%;
      }
      .audit-filters-actions .btn {
        flex: 1;
        min-width: 0;
      }
    }
  `]
})
export class HistorialEquipoInlineComponent implements OnInit {
  @Input() equipoId!: number;
  @Output() observacionClick = new EventEmitter<string>();
  items: HistorialEquipoDto[] = [];
  cargando = true;

  filtroTexto = '';
  filtroEstado = '';
  fechaDesde = '';
  fechaHasta = '';
  estadosPrestamo = ESTADOS_PRESTAMO;
  fpDesde?: flatpickr.Instance;
  fpHasta?: flatpickr.Instance;

  constructor(private http: HttpClient) {}

  get itemsFiltrados(): HistorialEquipoDto[] {
    const texto = this.filtroTexto.trim().toLowerCase();
    const desde = this.fechaDesde ? new Date(this.fechaDesde) : null;
    const hasta = this.fechaHasta ? new Date(this.fechaHasta + 'T23:59:59') : null;
    return this.items.filter(h => {
      if (texto) {
        const matchId = h.IdPrestamo?.toString().includes(texto) ?? false;
        const matchNombre = h.NombreUsuario?.toLowerCase().includes(texto) ?? false;
        if (!matchId && !matchNombre) return false;
      }
      if (this.filtroEstado && h.EstadoPrestamo !== this.filtroEstado) return false;
      if (desde || hasta) {
        const fecha = h.FechaPrestamo ? new Date(h.FechaPrestamo) : null;
        if (!fecha) return false;
        if (desde && fecha < desde) return false;
        if (hasta && fecha > hasta) return false;
      }
      return true;
    });
  }

  estadoEquipoLabel(estado?: string): string {
    switch (estado) {
      case 'operativo': return 'Operativo';
      case 'parcialmente_operativo': return 'Parcialmente operativo';
      case 'inoperativo': return 'Inoperativo';
      case 'pendiente': return 'Pendiente';
      case 'aprobado': return 'Aprobado';
      case 'activo': return 'Activo';
      case 'atrasado': return 'Atrasado';
      case 'finalizado': return 'Finalizado';
      case 'rechazado': return 'Rechazado';
      case 'cancelado': return 'Cancelado';
      default: return '—';
    }
  }

  estadoEquipoBadgeLabel(estado?: string): string {
    return estado ? this.estadoEquipoLabel(estado) : '—';
  }

  onFechaDesde(dates: Date[]) {
    this.fechaDesde = dates[0] ? dates[0].toISOString().split('T')[0] : '';
  }

  onFechaHasta(dates: Date[]) {
    this.fechaHasta = dates[0] ? dates[0].toISOString().split('T')[0] : '';
  }

  limpiarFiltros() {
    this.filtroTexto = '';
    this.filtroEstado = '';
    this.fechaDesde = '';
    this.fechaHasta = '';
    this.fpDesde?.clear();
    this.fpHasta?.clear();
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
