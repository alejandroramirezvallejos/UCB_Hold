import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';
import { Carrito } from '../../../models/carrito';
import { Prestamos } from '../../../models/admin/Prestamos';

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

    crearPrestamo(carrito: Carrito , carnet : string , contrato : (Blob | null)  ) {
      const grupoid : number [] = [];
  
      for( const [key,value] of Object.entries(carrito)) {
          if(carrito[Number(key)].cantidad>0){
            for(let i = 0 ; i < carrito[Number(key)].cantidad ; i++){
              grupoid.push(Number(key));
            }
          }
      }
  
  
  
      const formulario = {
        GrupoEquipoId: grupoid,
        FechaPrestamoEsperada: carrito[grupoid[0]].fecha_inicio,
        FechaDevolucionEsperada: carrito[grupoid[0]].fecha_final,
        CarnetUsuario : carnet,
        Contrato : null,
        Observacion : null,
      };
  
      return this.http.post(this.url, formulario);
    }

    eliminarPrestamo(id: number) {
    return this.http.delete(`${this.url}/${id}`);
    }


    cambiarEstadoPrestamo(Id : number, estado: string) {
    const APIurl = `${this.url}/estadoPrestamo`;
    const envio ={
      Id : Id,
      EstadoPrestamo : estado
    }

    return this.http.put(APIurl,envio)

    }


}
