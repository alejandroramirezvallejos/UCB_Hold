import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class CategoriasService {

  private apiurl = environment.apiUrl + '/api/Categoria';

  constructor(private http : HttpClient) { }

  obtenercategorias(){

    return this.http.get<any[]>(this.apiurl).pipe(
          map(data => data.map(item => ({
            name: item.nombre ,
          })))
        );


  }


}
