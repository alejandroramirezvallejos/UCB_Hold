import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { GrupoEquipo } from '../../models/grupo_equipo';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProductoService {
  private apiUrl = environment.apiUrl + '/api/GrupoEquipo';

  constructor(private http : HttpClient) { }

  // producto.service.ts
  getproducto(id: string): Observable<GrupoEquipo> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(item => ({
        id: item.id,
        descripcion: item.descripcion,
        nombre: item.nombre,
        modelo: ' ' + item.modelo || '',
        marca: ' ' + item.marca || '',
        url_data_sheet: item.urlData,
        link: item.urlImagen
      }))
    );
  }
  
}
