import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GrupoEquipo } from '../../../../models/grupo_equipo';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class GrupoEquipoService {

  private apiUrl = environment.apiUrl + '/api/GrupoEquipo/buscar';

  constructor(private http: HttpClient) { }

  getGrupoEquipo(categoria : string , producto : string): Observable<GrupoEquipo[]>  {
    var url : string   = this.apiUrl + '?nombre=' + producto + '&categoria=' + categoria;
    return this.http.get<any[]>(url).pipe(
      map(data => data.map(item => ({
        id: item.Id,
        nombre: item.Nombre ,
        descripcion : item.Descripcion  || '', 
        modelo: ' '+item.Modelo || '',
        url_data_sheet: item.UrlDataSheet || '' ,
        marca: ' '+item.Marca || '',
        link: item.UrlImagen ,

      })))
    );

  }

}
