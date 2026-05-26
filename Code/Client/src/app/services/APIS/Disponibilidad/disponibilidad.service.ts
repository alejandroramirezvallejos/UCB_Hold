import { Injectable } from '@angular/core';
import { Disponibilidad } from '../../../models/disponibilidad';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class DisponibilidadService {
  private url = environment.apiUrl + '/api/Carrito/disponibilidadEquipos';
  constructor(private http: HttpClient) {}
  private mapear(item: any): Disponibilidad {
    return {
      Fecha: item.Fecha ? new Date(item.Fecha) : null,
      IdGrupoEquipo: item.IdGrupoEquipo,
      CantidadDisponible: item.CantidadDisponible,
    } as Disponibilidad;
  }
  obtenerDisponibilidad(
    fechaInicio: Date,
    fechaFin: Date,
    grupoEquipoIds: number[],
  ) {
    const toLocalDate = (date: Date): string => {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}T00:00:00`;
    };

    const payload = {
      FechaInicio: toLocalDate(fechaInicio),
      FechaFin: toLocalDate(fechaFin),
      ArrayIds: grupoEquipoIds,
    };
    return this.http.post<any>(this.url, payload).pipe(
      map((response) => {
        const data =
          response.Value || response.value || response.data || response;
        return Array.isArray(data)
          ? data.map((item: any) => this.mapear(item))
          : [];
      }),
    );
  }
}
