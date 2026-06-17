import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { INLINE_SEARCH_STYLES } from './inline-search.styles';

@Component({
  selector: 'app-equipos-inline',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin equipos en este grupo</p> }
      @else {
        <div class="audit-filters">
          <div class="audit-search-wrap">
            <i class="fas fa-search search-icon"></i>
            <input type="text" [(ngModel)]="filtroTexto" placeholder="Buscar por código, serial o ubicación..." class="admin-search">
            @if (filtroTexto) {
              <button class="clear-search" (click)="filtroTexto = ''" title="Limpiar búsqueda" type="button">
                <i class="fas fa-times"></i>
              </button>
            }
          </div>
        </div>
        @if(itemsFiltrados.length === 0) {
          <p class="empty-inline">Ningún equipo coincide con la búsqueda</p>
        } @else {
          <table>
            <thead><tr><th>Código IMT</th><th>Estado</th><th>Serial</th><th>Ubicación</th></tr></thead>
            <tbody>
              @for(e of itemsFiltrados; track e.Id) {
                <tr>
                  <td><strong>IMT-{{ e.CodigoImt }}</strong></td>
                  <td><span [class]="'badge badge-' + estadoBadge(e.EstadoEquipo)">{{ e.EstadoEquipo }}</span></td>
                  <td>{{ e.NumeroSerial || '—' }}</td>
                  <td>{{ e.Ubicacion || '—' }}</td>
                </tr>
              }
            </tbody>
          </table>
        }
      }
    </div>
  `,
  styles: [INLINE_SEARCH_STYLES]
})
export class EquiposInlineComponent implements OnInit {
  @Input() grupoEquipoId!: number;
  items: any[] = [];
  cargando = true;
  filtroTexto = '';

  constructor(private readonly http: HttpClient) {}

  get itemsFiltrados(): any[] {
    const texto = this.filtroTexto.trim().toLowerCase();
    if (!texto) return this.items;
    return this.items.filter(e =>
      e.CodigoImt?.toString().toLowerCase().includes(texto) ||
      (e.NumeroSerial ?? '').toLowerCase().includes(texto) ||
      (e.Ubicacion ?? '').toLowerCase().includes(texto)
    );
  }

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Equipo/byGrupo/${this.grupoEquipoId}`).subscribe({
      next: res => { this.items = res?.Value ?? res?.value ?? []; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }

  estadoBadge(estado: string): string {
    if (!estado) return 'cancelado';
    if (estado.toLowerCase().includes('operativo') && !estado.toLowerCase().includes('parcial')) return 'activo';
    if (estado.toLowerCase().includes('parcial')) return 'pendiente';
    return 'rechazado';
  }
}
