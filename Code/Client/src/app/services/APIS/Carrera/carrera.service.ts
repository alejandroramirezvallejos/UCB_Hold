import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Carrera } from '../../../models/admin/Carreras';

@Injectable({
  providedIn: 'root'
})
export class CarreraService {
  private apiUrl = environment.apiUrl + '/api/Carrera'; 
  constructor(private http: HttpClient) { }

  obtenerCarreras() {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        id: item.Id,
        nombre: item.Nombre
      })))
    );
  }

  crearCarrera(carrera: string ) {
    const envio ={
      Nombre : carrera
    }

    return this.http.post<any>(this.apiUrl, envio);
  }

  actualizarCarrera(carrera : Carrera){
    const envio = {
      Nombre: carrera.Nombre
    }

    return this.http.put<any>(this.apiUrl + '/' + carrera.Id, envio);
  }

  eliminarCarrera(id: number){
    return this.http.delete<any>(this.apiUrl + '/' + id); 
  }



}
