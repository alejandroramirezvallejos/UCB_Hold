import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GaveteroService {
  private apiUrl = environment.apiUrl + '/api/Gavetero';
  constructor(private http: HttpClient) { }

  obtenerGaveteros() {
    return this.http.get(this.apiUrl);
  }


}
