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
        FechaEnvio: item.FechaEnvio,
        Leido: item.Leido
      })))
    );
  }


  leerNotificacion(id: string) {
    const url = `${this.apiUrl}/${id}/leida`;
    
    return this.http.post(url, null)

  }

  verificarnoleidas(carnet : string){
    const url = `${this.apiUrl}/${carnet}/tiene-no-leidas`;
    return this.http.get<{tieneNoLeidas : boolean}>(url).pipe(
      map(data => data.tieneNoLeidas)
    );

  }


  enviarretrasos(){
    const url = `${this.apiUrl}/enviar-retrasos`;
    return this.http.post(url, null);
  }

  enviarpenalizaciones(){
    const url = `${this.apiUrl}/enviar-penalizaciones`;
    return this.http.post(url, null);
  }

  enviarestadoprestamo(){
    const url = `${this.apiUrl}/enviar-estado-prestamo`;
    return this.http.post(url, null);
  }

  enviarnotificaciones(){
            this.enviarretrasos().subscribe({
          next: (data) => {

          }
          , error: (error) => {
            console.error("Error al enviar retrasos: ", error);
          }
        });

        this.enviarpenalizaciones().subscribe({
          next: (data) => {

          }
          , error: (error) => {
            console.error("Error al enviar penalizaciones: ", error);
          }
        });
        this.enviarestadoprestamo().subscribe({
          next: (data) => {

          }
          , error: (error) => {
            console.error("Error al enviar estado de préstamo: ", error);
          }
        });
        
  }



}
