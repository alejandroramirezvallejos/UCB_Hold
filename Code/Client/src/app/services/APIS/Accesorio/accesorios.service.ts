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
        id: item.Id,
        nombre: item.Nombre,
        modelo: item.Modelo,
        tipo: item.Tipo,
        descripcion: item.Descripcion,
        codigo_imt: item.CodigoImtEquipoAsociado,
        precio: item.Precio,
        url_data_sheet: item.UrlDataSheet,
        nombreEquipoAsociado: item.NombreEquipoAsociado
      })))
    );
  }

  crearAccesorio(accesorio: Accesorio) {
    const envio ={
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoIMT: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet
    }


    return this.http.post<any>(this.apiurl, envio);
  }

  eliminarAccesorio(id : number) {
    return this.http.delete(`${this.apiurl}/${id}`);

  }

  editarAccesorio(accesorio: Accesorio) {
    const envio = {
      Id : accesorio.id,
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoIMT: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet
    };

    return this.http.put(`${this.apiurl}`, envio );


}
}
