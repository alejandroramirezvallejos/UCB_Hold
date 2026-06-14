import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { INLINE_SEARCH_STYLES } from './inline-search.styles';

@Component({
  selector: 'app-gaveteros-inline',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin gaveteros en este mueble</p> }
      @else {
        <div class="audit-filters">
          <div class="audit-search-wrap">
            <i class="fas fa-search search-icon"></i>
            <input type="text" [(ngModel)]="filtroTexto" placeholder="Buscar por nombre..." class="admin-search">
            @if (filtroTexto) {
              <button class="clear-search" (click)="filtroTexto = ''" title="Limpiar búsqueda" type="button">
                <i class="fas fa-times"></i>
              </button>
            }
          </div>
        </div>
        @if(itemsFiltrados.length === 0) {
          <p class="empty-inline">Ningún gavetero coincide con la búsqueda</p>
        } @else {
          <table>
            <thead><tr><th>Nombre</th><th>Gaveteros</th></tr></thead>
            <tbody>
              @for(g of itemsFiltrados; track g.Id) {
                <tr>
                  <td>{{ g.Nombre }}</td>
                  <td>{{ g.NumeroGaveteros ?? 0 }}</td>
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
export class GaveterosInlineComponent implements OnInit {
  @Input() muebleId!: number;
  items: any[] = [];
  cargando = true;
  filtroTexto = '';

  constructor(private http: HttpClient) {}

  get itemsFiltrados(): any[] {
    const texto = this.filtroTexto.trim().toLowerCase();
    if (!texto) return this.items;
    return this.items.filter(g => (g.Nombre ?? '').toLowerCase().includes(texto));
  }

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Gavetero/byMueble/${this.muebleId}`).subscribe({
      next: res => { this.items = res?.Value ?? res?.value ?? []; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }
}
