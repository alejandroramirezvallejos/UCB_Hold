import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Equipos } from '../../../models/admin/Equipos';
@Injectable({
  providedIn: 'root'
})
export class EquipoService {
  private readonly apiUrl = environment.apiUrl+ '/api/Equipo';
  constructor(private readonly http: HttpClient) { }
  crearEquipo(equipo: any) {
    const envio = {
      IdGrupoEquipo: equipo.IdGrupoEquipo,
      CodigoUcb: equipo.CodigoUcb,
      Descripcion: equipo.Descripcion,
      NumeroSerial: equipo.NumeroSerial,
      Ubicacion: equipo.Ubicacion,
      Procedencia: equipo.Procedencia,
      CostoReferencia: equipo.CostoReferencia,
      TiempoMaximoPrestamo: equipo.TiempoMaximoPrestamo,
      IdGavetero: equipo.IdGavetero,
      EstadoEquipo: equipo.EstadoEquipo,
      FechaIngresoEquipo: equipo.FechaIngresoEquipo
    };
    return this.http.post<any>(this.apiUrl, envio);
  }
  obtenerEquipos(){
    return this.http.get<any>(this.apiUrl).pipe(
      map((data: any) => data.Value.map((item: any) => ({
        Id: item.Id,
        NombreGrupoEquipo: item.NombreGrupoEquipo,
        IdGrupoEquipo: item.IdGrupoEquipo,
        CodigoImt : item.CodigoImt,
        CodigoUcb: item.CodigoUcb,
        NumeroSerial: item.NumeroSerial,
        EstadoEquipo: item.EstadoEquipo,
        Ubicacion: item.Ubicacion,
        NombreGavetero: item.NombreGavetero,
        IdGavetero: item.IdGavetero,
        CostoReferencia: item.CostoReferencia,
        Descripcion: item.Descripcion,
        TiempoMaximoPrestamo: item.TiempoMaximoPrestamo,
        Procedencia: item.Procedencia,
        FechaIngresoEquipo: item.FechaIngresoEquipo,
      })))
    );
  }
  editarEquipo(equipo: Equipos) {
    const envio = {
      Id: equipo.Id,
      IdGrupoEquipo: equipo.IdGrupoEquipo,
      CodigoUcb: equipo.CodigoUcb,
      Descripcion: equipo.Descripcion,
      EstadoEquipo: equipo.EstadoEquipo,
      NumeroSerial: equipo.NumeroSerial,
      Ubicacion: equipo.Ubicacion,
      Procedencia: equipo.Procedencia,
      CostoReferencia: equipo.CostoReferencia,
      TiempoMaximoPrestamo: equipo.TiempoMaximoPrestamo,
      IdGavetero: equipo.IdGavetero,
      FechaIngresoEquipo: equipo.FechaIngresoEquipo
    };
    return this.http.put<any>(`${this.apiUrl}/${equipo.Id}`, envio);
  }
  eliminarEquipo(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
