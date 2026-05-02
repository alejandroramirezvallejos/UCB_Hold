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
        Id: item.Id,
        Nombre: item.Nombre,
        Modelo: item.Modelo,
        Marca: item.Marca,
        Cantidad: item.Cantidad,
        IdCategoria: item.IdCategoria
      })))
    );
  }
  getGrupoEquipo(categoria : string , producto : string): Observable<GrupoEquipo[]>  {
    var url : string   = this.apiUrl +'/buscar' + '?nombre=' + producto + '&categoria=' + categoria;
    return this.http.get<any[]>(url).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        Nombre: item.Nombre,
        Modelo: item.Modelo || '',
        Marca: item.Marca || '',
        Cantidad: item.Cantidad || 0,
        IdCategoria: item.IdCategoria
      })))
    );
  }
  getproducto(id: string): Observable<GrupoEquipo> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(item => ({
        Id: item.Id,
        Nombre: item.Nombre,
        Modelo: item.Modelo || '',
        Marca: item.Marca || '',
        Cantidad: item.Cantidad || 0,
        IdCategoria: item.IdCategoria
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
