import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

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


}
