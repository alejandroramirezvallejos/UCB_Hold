import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-equipos-gavetero-inline',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="inline-panel">
      @if(cargando) { <p class="empty-inline">Cargando...</p> }
      @else if(items.length === 0) { <p class="empty-inline">Sin equipos en este gavetero</p> }
      @else {
        <table>
          <thead><tr><th>Código IMT</th><th>Estado</th><th>Serial</th></tr></thead>
          <tbody>
            @for(e of items; track e.Id) {
              <tr>
                <td><strong>IMT-{{ e.CodigoImt }}</strong></td>
                <td>{{ e.EstadoEquipo }}</td>
                <td>{{ e.NumeroSerial || '—' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </div>
  `
})
export class EquiposGaveteroInlineComponent implements OnInit {
  @Input() gaveteroId!: number;
  items: any[] = [];
  cargando = true;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/api/Equipo/byGavetero/${this.gaveteroId}`).subscribe({
      next: res => { this.items = res?.Value ?? res?.value ?? []; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }
}
