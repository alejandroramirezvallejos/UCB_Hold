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

  obtenerDisponibilidad(fechaInicio : Date , fechaFin : Date , grupoEquipoIds : number[]){ 
    var envio = this.url + '?FechaInicio=' + fechaInicio.toISOString().split('Z')[0] + '&FechaFin=' +  fechaFin.toISOString().split('Z')[0]+ '&ArrayIds=' + grupoEquipoIds.join(',');
    return this.http.get<any[]>(envio).pipe(
      map(data=> data.map(item => this.mapear(item)))
    );

  };




}
