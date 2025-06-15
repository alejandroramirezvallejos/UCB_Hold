import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MantenimientoService {
  private apiUrl = environment.apiUrl + '/api/Mantenimiento'; 
  constructor(private http: HttpClient) { }

  obtenerMantenimientos() {
    return this.http.get(this.apiUrl);
  }

}
