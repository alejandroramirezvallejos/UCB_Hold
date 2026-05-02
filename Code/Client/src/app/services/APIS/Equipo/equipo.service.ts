import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Equipos } from '../../../models/admin/Equipos';
@Injectable({
  providedIn: 'root'
})
export class EquipoService {
  private apiUrl = environment.apiUrl+ '/api/Equipo';
  constructor(private http: HttpClient) { }
  crearEquipo(equipo: any) {
    const envio = {
      NombreGrupoEquipo: equipo.NombreGrupoEquipo,
      Modelo: equipo.Modelo,
      Marca: equipo.Marca,
      CodigoUcb: equipo.CodigoUcb,
      Descripcion: equipo.Descripcion,
      NumeroSerial: equipo.NumeroSerial,
      Ubicacion: equipo.Ubicacion,
      Procedencia: equipo.Procedencia,
      CostoReferencia: equipo.CostoReferencia,
      TiempoMaximoPrestamo: equipo.TiempoMaximoPrestamo,
      NombreGavetero: equipo.NombreGavetero,
      EstadoEquipo: equipo.EstadoEquipo
    };
    return this.http.post<any>(this.apiUrl, envio);
  }
  obtenerEquipos(){
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        CodigoUcb: item.CodigoUcb,
        CodigoImt: item.CodigoImt,
        Descripcion: item.Descripcion,
        NumeroSerial: item.NumeroSerial,
        Ubicacion: item.Ubicacion,
        CostoReferencia: item.CostoReferencia,
        TiempoMaxPrestamo: item.TiempoMaxPrestamo,
        Procedencia: item.Procedencia,
        IdGavetero: item.IdGavetero,
        IdGrupoEquipo: item.IdGrupoEquipo,
        EstadoEquipo: item.EstadoEquipo
      })))
    );
  }
  editarEquipo(equipo: Equipos) {
    const envio = {
      Id: equipo.Id,
      NombreGrupoEquipo: equipo.NombreGrupoEquipo,
      Modelo: equipo.Modelo,
      Marca: equipo.Marca,
      CodigoImt: equipo.CodigoImt,
      CodigoUcb: equipo.CodigoUcb,
      Descripcion: equipo.Descripcion,
      EstadoEquipo: equipo.EstadoEquipo,
      NumeroSerial: equipo.NumeroSerial,
      Ubicacion: equipo.Ubicacion,
      Procedencia: equipo.Procedencia,
      CostoReferencia: equipo.CostoReferencia,
      TiempoMaximoPrestamo: equipo.TiempoMaximoPrestamo,
      NombreGavetero: equipo.NombreGavetero
    };
    return this.http.put<any>(this.apiUrl, envio);
  }
  eliminarEquipo(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
