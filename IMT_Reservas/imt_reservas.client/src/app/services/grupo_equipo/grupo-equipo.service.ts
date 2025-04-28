import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GrupoEquipoService {

  private apiUrl = environment.apiUrl + '/api/productos';

  constructor(private http: HttpClient) { }

  getGrupoEquipo() {
   return this.http.get<any[]>(this.apiUrl);

  }

}
