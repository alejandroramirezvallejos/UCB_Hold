import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-prestamos-inline',
  standalone: true,
  imports: [CommonModule, DatePipe],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin préstamos registrados</p> }
      @else {
        <table>
          <thead><tr><th>ID</th><th>Equipos</th><th>Estado</th><th>Solicitud</th><th>Devolución Esperada</th></tr></thead>
          <tbody>
            @for(p of items; track p.Id) {
              <tr>
                <td>#{{ p.Id }}</td>
                <td>{{ p.NombreGrupoEquipo || '—' }}</td>
                <td><span [class]="'badge badge-' + (p.EstadoPrestamo || 'cancelado')">{{ p.EstadoPrestamo }}</span></td>
                <td>{{ p.FechaSolicitud | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
                <td>{{ p.FechaDevolucionEsperada | date:'dd/MM/yyyy':'America/La_Paz' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </div>
  `
})
export class PrestamosInlineComponent implements OnInit {
  @Input() carnet!: string;
  items: any[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Prestamo/byUsuario/${this.carnet}`).subscribe({
      next: res => {
        const data = res?.Value ?? res?.value ?? [];
        this.items = Array.isArray(data) ? data : [];
        this.cargando = false;
      },
      error: () => { this.cargando = false; }
    });
  }
}
