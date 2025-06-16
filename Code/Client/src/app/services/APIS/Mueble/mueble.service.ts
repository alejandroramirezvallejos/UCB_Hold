import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Muebles } from '../../../models/admin/Muebles';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MuebleService {
  private apiUrl = environment.apiUrl + '/api/Mueble'; 

  constructor(private http: HttpClient) { }

// FALTAN COSAS EN EL SWAGER PARECE
  crearMueble(mueble : Muebles){
    const enviar={
      Nombre: mueble.Nombre,
      Tipo: mueble.Tipo,
      Costo: mueble.Costo,
      Ubicacion: mueble.Ubicacion,
      Longitud: mueble.Longitud,
      Profundidad: mueble.Profundidad,
      Altura: mueble.Altura
    };
    
    return this.http.post(this.apiUrl, enviar);
  }


  obtenerMuebles() {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        Nombre: item.Nombre,
        NumeroGaveteros: item.NumeroGaveteros,
        Ubicacion: item.Ubicacion,
        Tipo: item.Tipo,
        Costo: item.Costo,
        Longitud: item.Longitud,
        Profundidad: item.Profundidad,
        Altura: item.Altura
      })))
    );
  }


  actualizarMueble(mueble: Muebles) {
    const enviar = {
      Nombre: mueble.Nombre,
      Tipo: mueble.Tipo,
      Costo: mueble.Costo,
      Ubicacion: mueble.Ubicacion,
      Longitud: mueble.Longitud,
      Profundidad: mueble.Profundidad,
      Altura: mueble.Altura
    };

    return this.http.put(`${this.apiUrl}/${mueble.Id}`, enviar);
  }


  eliminarMueble(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

}
 