import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { Accesorio } from '../../../models/admin/Accesorio';
@Injectable({
  providedIn: 'root',
})
export class AccesoriosService {
  private apiurl = environment.apiUrl + '/api/Accesorio';
  constructor(private http: HttpClient) {}
  obtenerAccesorios() {
    return this.http.get<any>(this.apiurl).pipe(
      map((data: any) =>
        data.Value.map((item: any) => ({
          Id: item.Id,
          Nombre: item.Nombre,
          Modelo: item.Modelo,
          Tipo: item.Tipo,
          Descripcion: item.Descripcion,
          CodigoImtEquipoAsociado: item.CodigoImtEquipoAsociado,
          Precio: item.Precio,
          UrlDataSheet: item.UrlDataSheet,
          IdEquipo: item.IdEquipo,
          NombreEquipoAsociado: item.NombreEquipoAsociado,
        })),
      ),
    );
  }
  crearAccesorio(accesorio: Accesorio) {
    const envio = {
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoImtEquipoAsociado: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.id_equipo,
    };
    return this.http.post<any>(this.apiurl, envio);
  }
  eliminarAccesorio(id: number) {
    return this.http.delete(`${this.apiurl}/${id}`);
  }
  editarAccesorio(accesorio: Accesorio) {
    const envio = {
      Id: accesorio.Id,
      Nombre: accesorio.nombre,
      Modelo: accesorio.modelo,
      Tipo: accesorio.tipo,
      CodigoImtEquipoAsociado: accesorio.codigo_imt,
      Descripcion: accesorio.descripcion,
      Precio: accesorio.precio,
      UrlDataSheet: accesorio.url_data_sheet,
      IdEquipo: accesorio.id_equipo,
    };
    return this.http.put(`${this.apiurl}/${accesorio.Id}`, envio);
  }
}
