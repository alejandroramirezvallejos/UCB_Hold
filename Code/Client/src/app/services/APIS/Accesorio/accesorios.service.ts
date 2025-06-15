import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

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
        nombreEquipoAsociado: item.NombreEquipoAsociado
      })))
    );
  }

}
