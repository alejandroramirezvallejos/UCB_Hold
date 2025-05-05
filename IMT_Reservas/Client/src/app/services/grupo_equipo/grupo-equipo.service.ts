import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GrupoEquipo } from '../../models/grupo_equipo';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class GrupoEquipoService {

  private apiUrl = environment.apiUrl + '/api/Equipos';

  constructor(private http: HttpClient) { }

  getGrupoEquipo(): Observable<GrupoEquipo[]>  {
    

    return this.http.get<GrupoEquipo[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        id: item.id,
        nombre: item.nombre ,
        modelo: ' '+item.modelo || '',
        marca: ' '+item.marca || '',
        url_data_sheet: item.url_data_sheet ,
        link: item.link 
      })))
    );


    
  }

}
