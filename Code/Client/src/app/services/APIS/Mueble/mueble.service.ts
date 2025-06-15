import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MuebleService {
  private apiUrl = environment.apiUrl + '/api/Mueble'; 

  constructor(private http: HttpClient) { }

  obtenerMuebles() {
    return this.http.get(this.apiUrl);
  }

}
