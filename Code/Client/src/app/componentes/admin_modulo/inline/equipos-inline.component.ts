import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-equipos-inline',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin equipos en este grupo</p> }
      @else {
        <table>
          <thead><tr><th>Código IMT</th><th>Estado</th><th>Serial</th><th>Ubicación</th></tr></thead>
          <tbody>
            @for(e of items; track e.Id) {
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
    </div>
  `
})
export class EquiposInlineComponent implements OnInit {
  @Input() grupoEquipoId!: number;
  items: any[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

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
