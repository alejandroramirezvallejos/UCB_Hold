import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Categorias } from '../../../models/admin/Categorias';

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

  crearCategoria(categoria : Categorias){

    const envio ={
      Nombre : categoria.Nombre
    }

    return this.http.post<any>(this.apiurl, envio);
  }

  actualizarCategoria(categoria : Categorias){
    const envio = {
      Id: categoria.Id,
      Nombre: categoria.Nombre
    }

    return this.http.put<any>(this.apiurl, envio);
  }

  eliminarCategoria(id: number){
    return this.http.delete<any>(this.apiurl + '/' + id);
  }

}
