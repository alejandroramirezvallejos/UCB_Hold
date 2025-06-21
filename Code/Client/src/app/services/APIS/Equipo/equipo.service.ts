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


  // AREGLAR PIDE MODELO Y MARCA Y EN EL OTRO NO 
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
      NombreGavetero: equipo.NombreGavetero
    };

    return this.http.post<any>(this.apiUrl, envio);
  }


  obtenerEquipos(){
    
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        NombreGrupoEquipo: item.NombreGrupoEquipo,
        Modelo: item.Modelo,
        Marca: item.Marca,
        CodigoImt : item.CodigoImt,
        CodigoUcb: item.CodigoUcb,
        NumeroSerial: item.NumeroSerial,
        EstadoEquipo: item.EstadoEquipo,
        Ubicacion: item.Ubicacion,
        NombreGavetero: item.NombreGavetero,
        CostoReferencia: item.CostoReferencia,
        Descripcion: item.Descripcion,
        TiempoMaximoPrestamo: item.TiempoMaximoPrestamo,
        Procedencia: item.Procedencia,

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
