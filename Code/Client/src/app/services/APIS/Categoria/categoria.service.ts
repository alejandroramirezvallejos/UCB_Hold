import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoriaService {

  private apiurl = environment.apiUrl + '/api/Categoria';

  constructor(private http : HttpClient) { }

  obtenercategorias(){

    return this.http.get<any[]>(this.apiurl).pipe(
          map(data => data.map(item => ({
            id : item.Id,
            nombre: item.Nombre,
          })))
        );


  }

}
