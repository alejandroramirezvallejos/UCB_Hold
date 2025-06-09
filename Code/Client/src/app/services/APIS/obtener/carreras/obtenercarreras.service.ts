import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ObtenercarrerasService {
  private apiUrl = environment.apiUrl + '/api/Carrera'; 
  constructor(private http: HttpClient) { }

  obtenerCarreras() {
    return this.http.get<string[]>(this.apiUrl);
  }
}
