import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Componente } from '../../../models/admin/Componente';
import { map } from 'rxjs';




@Injectable({
  providedIn: 'root'
})
export class ComponenteService {
  private apiUrl = environment.apiUrl + '/api/Componente';

  constructor(private http : HttpClient) { }
  crearComponente(componente: Componente) {
    const envio = {
      Nombre : componente.Nombre,
      Modelo: componente.Modelo,
      Tipo: componente.Tipo,
      CodigoIMT : componente.CodigoImtEquipo,
      Descripcion: componente.Descripcion,
      PrecioReferencia: componente.PrecioReferencia,
      UrlDataSheet: componente.UrlDataSheet
    }

    return this.http.post<any>(this.apiUrl, envio);
  }



  obtenerComponentes (){
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        Nombre: item.Nombre,
        Modelo: item.Modelo,
        Tipo: item.Tipo,
        Descripcion: item.Descripcion,
        PrecioReferencia: item.PrecioReferencia,
        NombreEquipo: item.NombreEquipo,
        CodigoImtEquipo: item.CodigoImtEquipo,
        UrlDataSheet: item.UrlDataSheet
      })))
    );

    
  }

  actualizarComponente(componente: Componente) {
    const envio = {
      Nombre: componente.Nombre,
      Modelo: componente.Modelo,
      Tipo: componente.Tipo,
      CodigoIMT: componente.CodigoImtEquipo,
      Descripcion: componente.Descripcion,
      PrecioReferencia: componente.PrecioReferencia,
      UrlDataSheet: componente.UrlDataSheet
    }

    return this.http.put<any>(this.apiUrl + '/' + componente.Id, envio);
  }

  eliminarComponente(id: number) {
    return this.http.delete<any>(this.apiUrl + '/' + id);
  }


}
