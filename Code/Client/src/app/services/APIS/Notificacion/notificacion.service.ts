import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificacionService {
  apiUrl = environment.apiUrl + '/api/Notificacion';
  constructor(private http : HttpClient) { }


  obtenerNotificaciones(carnet: string) {
    const url = `${this.apiUrl}/${carnet}`;
    return this.http.get<any[]>(url).pipe(
      map(data=> data.map(  item => ({
        Id: item.Id,
        CarnetUsuario: item.CarnetUsuario,
        Titulo: item.Titulo,
        Contenido: item.Contenido,
        FechaEnvio: item.FechaEnvio
      })))
    );
  }


  leerNotificacion(id: string) {
    const url = `${this.apiUrl}/${id}/leida`;
    
    return this.http.post(url, null)

  }


}
