import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Accesorio } from '../../../models/admin/Accesorio';
@Injectable({
  providedIn: 'root'
})
export class AccesoriosService {
    private apiurl = environment.apiUrl + '/api/Accesorio'; 
  constructor(private http : HttpClient) { }
  obtenerAccesorios() {
    return this.http.get<any[]>(this.apiurl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        nombre: item.nombre,
        modelo: item.modelo,
        tipo: item.tipo,
        precio: item.precio,
        descripcion: item.descripcion,
        codigo_imt: item.codigo_imt,
        url_data_sheet: item.url_data_sheet,
        nombreEquipoAsociado: item.nombreEquipoAsociado
      })))
    );
  }
  crearAccesorio(accesorio: Accesorio) {
    const envio ={
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.codigo_imt || 0
    }
    return this.http.post<any>(this.apiurl, envio);
  }
  eliminarAccesorio(id : number) {
    return this.http.delete(`${this.apiurl}/${id}`);
  }
  editarAccesorio(accesorio: Accesorio) {
    const envio = {
      Id : accesorio.Id,
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.codigo_imt || 0
    };
    return this.http.put(`${this.apiurl}`, envio );
}
}
