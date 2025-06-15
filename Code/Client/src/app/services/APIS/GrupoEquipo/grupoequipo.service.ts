import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { GrupoEquipo } from '../../../models/grupo_equipo';

@Injectable({
  providedIn: 'root'
})
export class GrupoequipoService {

  private apiUrl = environment.apiUrl + '/api/GrupoEquipo';

  constructor(private http: HttpClient) { }

  getGrupoEquipo(categoria : string , producto : string): Observable<GrupoEquipo[]>  {
    var url : string   = this.apiUrl +'/buscar' + '?nombre=' + producto + '&categoria=' + categoria;
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

  getproducto(id: string): Observable<GrupoEquipo> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(item => ({
        id: item.Id,
        descripcion: item.Descripcion,
        nombre: item.Nombre,
        modelo: ' ' + item.Modelo || '',
        marca: ' ' + item.Marca || '',
        url_data_sheet: item.UrlData,
        link: item.UrlImagen
      }))
    );
  }



  
}
