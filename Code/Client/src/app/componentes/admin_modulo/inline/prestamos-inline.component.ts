import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { INLINE_SEARCH_STYLES } from './inline-search.styles';

@Component({
  selector: 'app-prestamos-inline',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin préstamos registrados</p> }
      @else {
        <div class="audit-filters">
          <div class="audit-search-wrap">
            <i class="fas fa-search search-icon"></i>
            <input type="text" [(ngModel)]="filtroTexto" placeholder="Buscar por # préstamo o equipo..." class="admin-search">
            @if (filtroTexto) {
              <button class="clear-search" (click)="filtroTexto = ''" title="Limpiar búsqueda" type="button">
                <i class="fas fa-times"></i>
              </button>
            }
          </div>
        </div>
        @if(itemsFiltrados.length === 0) {
          <p class="empty-inline">Ningún préstamo coincide con la búsqueda</p>
        } @else {
          <table>
            <thead><tr><th>ID</th><th>Equipos</th><th>Estado</th><th>Solicitud</th><th>Devolución Esperada</th></tr></thead>
            <tbody>
              @for(p of itemsFiltrados; track p.Id) {
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
      }
    </div>
  `,
  styles: [INLINE_SEARCH_STYLES]
})
export class PrestamosInlineComponent implements OnInit {
  @Input() carnet!: string;
  items: any[] = [];
  cargando = true;
  filtroTexto = '';

  constructor(private http: HttpClient) {}

  get itemsFiltrados(): any[] {
    const texto = this.filtroTexto.trim().toLowerCase();
    if (!texto) return this.items;
    return this.items.filter(p =>
      p.Id?.toString().includes(texto) ||
      (p.NombreGrupoEquipo ?? '').toLowerCase().includes(texto)
    );
  }

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
