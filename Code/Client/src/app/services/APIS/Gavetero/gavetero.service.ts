import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Gaveteros } from '../../../models/admin/Gaveteros';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GaveteroService {
  private apiUrl = environment.apiUrl + '/api/Gavetero';
  constructor(private http: HttpClient) { }


  crearGavetero(gavetero: Gaveteros) {
    const envio = {
      Nombre: gavetero.Nombre,
      Tipo: gavetero.Tipo,
      NombreMueble: gavetero.NombreMueble,
      Longitud: gavetero.Longitud,
      Profundidad: gavetero.Profundidad,
      Altura: gavetero.Altura
    };

    return this.http.post<any>(this.apiUrl, envio);
  }


  obtenerGaveteros() {
    return this.http.get<Gaveteros[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        Nombre: item.Nombre,
        Tipo: item.Tipo,
        NombreMueble: item.NombreMueble,
        Longitud: item.Longitud,
        Profundidad: item.Profundidad,
        Altura: item.Altura
      })))
    );
  }

  editarGavetero(gavetero: Gaveteros) {
    const envio = {
      Id: gavetero.Id,
      Nombre: gavetero.Nombre,
      Tipo: gavetero.Tipo,
      NombreMueble: gavetero.NombreMueble,
      Longitud: gavetero.Longitud,
      Profundidad: gavetero.Profundidad,
      Altura: gavetero.Altura
    };

    return this.http.put<any>(this.apiUrl, envio);
  }

  eliminarGavetero(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

}
