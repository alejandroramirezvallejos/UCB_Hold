import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Accesorio } from '@entities/admin';
import { ApiResponse, extractApiValue } from '@shared/api';
import { AccesorioApiItem } from './accesorio-api-item';
@Injectable({
  providedIn: 'root',
})
export class AccesoriosService {
  private readonly apiurl = environment.apiUrl + '/api/Accesorio';
  constructor(private readonly http: HttpClient) {}

  obtenerAccesorios(): Observable<Accesorio[]> {
    return this.http.get<ApiResponse<AccesorioApiItem[]>>(this.apiurl).pipe(
      map((data) =>
        extractApiValue(data, []).map((item) => ({
          Id: item.Id,
          nombre: item.Nombre,
          modelo: item.Modelo,
          tipo: item.Tipo,
          descripcion: item.Descripcion,
          codigo_imt: item.CodigoImtEquipoAsociado,
          precio: item.Precio,
          url_data_sheet: item.UrlDataSheet,
          id_equipo: item.IdEquipo,
          nombreEquipoAsociado: item.NombreEquipoAsociado,
        })),
      ),
    );
  }

  crearAccesorio(accesorio: Accesorio) {
    const envio = {
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoImtEquipoAsociado: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.id_equipo,
    };
    return this.http.post<unknown>(this.apiurl, envio);
  }

  eliminarAccesorio(id: number) {
    return this.http.delete<unknown>(`${this.apiurl}/${id}`);
  }

  editarAccesorio(accesorio: Accesorio) {
    const envio = {
      Id: accesorio.Id,
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoImtEquipoAsociado: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.id_equipo,
    };
    return this.http.put<unknown>(`${this.apiurl}/${accesorio.Id}`, envio);
  }
}
