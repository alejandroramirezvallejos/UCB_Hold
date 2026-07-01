import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Carrera } from '@entities/admin';
import { ApiResponse, extractApiValue } from '@shared/api';
import { CarreraApiItem } from './carrera-api-item';
@Injectable({
  providedIn: 'root',
})
export class CarreraService {
  private readonly apiUrl = environment.apiUrl + '/api/Carrera';
  constructor(private readonly http: HttpClient) {}

  obtenerCarreras(): Observable<Carrera[]> {
    return this.http.get<ApiResponse<CarreraApiItem[]>>(this.apiUrl).pipe(
      map((response) =>
        extractApiValue(response, []).map((item) => ({
          Id: item.Id,
          Nombre: item.Nombre,
        })),
      ),
    );
  }

  crearCarrera(carrera: string) {
    const envio = {
      Nombre: carrera,
    };
    return this.http.post<unknown>(this.apiUrl, envio);
  }

  actualizarCarrera(carrera: Carrera) {
    const envio = {
      Id: carrera.Id,
      Nombre: carrera.Nombre,
    };
    return this.http.put<unknown>(`${this.apiUrl}/${carrera.Id}`, envio);
  }

  eliminarCarrera(id: number) {
    return this.http.delete<unknown>(this.apiUrl + '/' + id);
  }
}
