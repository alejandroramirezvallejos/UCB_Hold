import { Injectable } from '@angular/core';
import { Disponibilidad } from '../../../models/disponibilidad';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class DisponibilidadService {
  private url = environment.apiUrl + '/api/Carrito/disponibilidadEquipos'; 
  constructor(private http : HttpClient) { }
  private mapear(item : any): Disponibilidad{
    return {
        Fecha: item.Fecha ? new Date(item.Fecha) : null,
        IdGrupoEquipo: item.IdGrupoEquipo,
        CantidadDisponible: item.CantidadDisponible
    } as Disponibilidad;  
  }
  obtenerDisponibilidad(fechaInicio: Date, fechaFin: Date, grupoEquipoIds: number[]) {
    const payload = {
      FechaInicio: fechaInicio.toISOString(),
      FechaFin: fechaFin.toISOString(),
      ArrayIds: grupoEquipoIds
    };
    return this.http.post<any>(this.url, payload).pipe(
      map(response => {
        const data = response.data?.value || response.value || response.data || response;
        return Array.isArray(data) ? data.map((item: any) => this.mapear(item)) : [];
      })
    );
  }
}
