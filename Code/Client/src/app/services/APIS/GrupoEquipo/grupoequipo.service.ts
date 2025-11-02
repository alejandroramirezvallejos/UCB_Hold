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


  crearGrupoEquipo(grupoEquipo: GrupoEquipo){
    const envio = {
      Nombre: grupoEquipo.nombre,
      Modelo: grupoEquipo.modelo,
      Marca: grupoEquipo.marca,
      NombreCategoria: grupoEquipo.nombreCategoria,
      Descripcion: grupoEquipo.descripcion,
      UrlDataSheet: grupoEquipo.url_data_sheet,
      UrlImagen: grupoEquipo.link
    };

    return this.http.post<any>(this.apiUrl, envio);

  }

  obtenersinfiltroGruposEquipos() {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        id: item.Id,
        nombre: item.Nombre ,
        descripcion : item.Descripcion  , 
        modelo: item.Modelo ,
        url_data_sheet: item.UrlDataSheet  ,
        marca: item.Marca ,
        link: item.UrlImagen ,
        nombreCategoria: item.NombreCategoria ,
        Cantidad: item.Cantidad,
        CostoPromedio: item.CostoPromedio
      })))
    );
  }



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
        nombreCategoria: item.NombreCategoria || '',
        Cantidad: item.Cantidad || 0,
        CostoPromedio: item.CostoPromedio || 0

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
        url_data_sheet: item.UrlDataSheet,
        link: item.UrlImagen,
        Cantidad : item.Cantidad || 0,
        CostoPromedio : item.CostoPromedio || 0
      }))
    );
  }


  editarGrupoEquipo(grupoEquipo: GrupoEquipo) {
    const envio = {
      Id: grupoEquipo.id,
      Nombre: grupoEquipo.nombre,
      Modelo: grupoEquipo.modelo,
      Marca: grupoEquipo.marca,
      NombreCategoria: grupoEquipo.nombreCategoria,
      Descripcion: grupoEquipo.descripcion,
      UrlDataSheet: grupoEquipo.url_data_sheet,
      UrlImagen: grupoEquipo.link
    };

    return this.http.put<any>(this.apiUrl, envio);
  }


  eliminarGrupoEquipo(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

}
