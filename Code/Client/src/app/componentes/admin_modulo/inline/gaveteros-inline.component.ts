import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-gaveteros-inline',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin gaveteros en este mueble</p> }
      @else {
        <table>
          <thead><tr><th>Nombre</th><th>Gaveteros</th></tr></thead>
          <tbody>
            @for(g of items; track g.Id) {
              <tr>
                <td>{{ g.Nombre }}</td>
                <td>{{ g.NumeroGaveteros ?? 0 }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </div>
  `
})
export class GaveterosInlineComponent implements OnInit {
  @Input() muebleId!: number;
  items: any[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Gavetero/byMueble/${this.muebleId}`).subscribe({
      next: res => { this.items = res?.Value ?? res?.value ?? []; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }
}
