import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PrestamosAPIService {

  private url = environment.apiUrl + '/api/Prestamo'; 
  constructor(private http : HttpClient) { }


  
  obtenerPrestamos() {
    return this.http.get<any[]>(this.url).pipe(
      map(data => data.map(item => ({
          Id: item.Id,
        CarnetUsuario: item.CarnetUsuario,
        NombreUsuario: item.NombreUsuario,
        ApellidoPaternoUsuario: item.ApellidoPaternoUsuario,
        TelefonoUsuario: item.TelefonoUsuario,
        NombreGrupoEquipo: item.NombreGrupoEquipo,
        CodigoImt: item.CodigoImt,
        FechaSolicitud: item.FechaSolicitud,
        FechaPrestamoEsperada: item.FechaPrestamoEsperada,
        FechaPrestamo: item.FechaPrestamo,
        FechaDevolucionEsperada: item.FechaDevolucionEsperada,
        FechaDevolucion: item.FechaDevolucion,
        Observacion: item.observacion,
        EstadoPrestamo: item.EstadoPrestamo
      })))
    );
  }

}
