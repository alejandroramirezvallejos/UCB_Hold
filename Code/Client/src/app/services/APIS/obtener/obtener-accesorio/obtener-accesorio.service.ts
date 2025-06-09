import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ObtenerAccesorioService {
  private apiurl = environment.apiUrl + '/api/Accesorio'; 
  constructor(private http : HttpClient) { }

  obtenerAccesorios() {
    return this.http.get<any[]>(this.apiurl).pipe(
      map(data => data.map(item => ({
        id: item.id,
        nombre: item.nombre,
        modelo: item.modelo,
        tipo: item.tipo,
        descripcion: item.descripcion,
        codigo_imt: item.codigoImtEquipoAsociado,
        precio: item.precio,
        nombreEquipoAsociado: item.nombreEquipoAsociado
      })))
    );
  }
  }


