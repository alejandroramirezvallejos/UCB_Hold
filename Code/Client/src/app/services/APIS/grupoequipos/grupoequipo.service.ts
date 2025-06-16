import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GrupoequipoService {
 private url = environment.apiUrl + '/api/Prestamo'; 
  constructor(private http: HttpClient) { }

  obtenreGrupoEquipos() {
    return this.http.get<any[]>(this.url).pipe(
      map(data => data.map(item => ({
        id: item.Id,
        nombre: item.Nombre,
        descripcion: item.Descripcion,
        modelo: item.Modelo,
        url_data_sheet: item.Url_data_sheet,
        marca: item.Marca,
        link: item.Link
      })))
    );

  }
}
