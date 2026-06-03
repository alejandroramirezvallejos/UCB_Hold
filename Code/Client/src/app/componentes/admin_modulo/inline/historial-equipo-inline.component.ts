import { Component, Input, OnInit } from '@angular/core';
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
              <th>Dev. Esperada</th>
              <th>Devuelto</th>
              <th>Estado</th>
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
                <td>{{ h.FechaDevolucionEsperada | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                <td>{{ h.FechaDevolucion ? (h.FechaDevolucion | date:'dd/MM/yyyy':'America/La_Paz') : '—' }}</td>
                <td><span [class]="'badge badge-' + (h.EstadoPrestamo || 'cancelado')">{{ h.EstadoPrestamo }}</span></td>
                <td>{{ h.Observacion || '—' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </div>
  `
})
export class HistorialEquipoInlineComponent implements OnInit {
  @Input() equipoId!: number;
  items: HistorialEquipoDto[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

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
