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


  private mapearPrestamo(item: any): Prestamos {
    return {
       Id: item.Id,
        CarnetUsuario: item.CarnetUsuario,
        NombreUsuario: item.NombreUsuario,
        ApellidoPaternoUsuario: item.ApellidoPaternoUsuario,
        TelefonoUsuario: item.TelefonoUsuario,
        NombreGrupoEquipo: item.NombreGrupoEquipo,
        CodigoImt: item.CodigoImt,
        FechaSolicitud: item.FechaSolicitud ? new Date(item.FechaSolicitud) : null,
        FechaPrestamoEsperada: item.FechaPrestamoEsperada ? new Date(item.FechaPrestamoEsperada) : null,
        FechaPrestamo: item.FechaPrestamo ? new Date(item.FechaPrestamo) : null,
        FechaDevolucionEsperada: item.FechaDevolucionEsperada ? new Date(item.FechaDevolucionEsperada) : null,
        FechaDevolucion: item.FechaDevolucion ? new Date(item.FechaDevolucion) : null,
        Observacion: item.Observacion,
        EstadoPrestamo: item.EstadoPrestamo,
        IdContrato: item.IdContrato,
        FileId: item.FileId,
         Ubicacion_Equipo : item. Ubicacion_Equipo,
        Nombre_Gavetero : item.Nombre_Gavetero,
        Nombre_Mueble : item.Nombre_Mueble,
        Ubicacion_Mueble : item.Ubicacion_Mueble

    } as Prestamos;
  }

  
  obtenerPrestamos() {
    return this.http.get<any[]>(this.url).pipe(
      map(data => data.map(item => this.mapearPrestamo(item)))
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
      
      const formData = new FormData();
      
       grupoid.forEach(id => {
        formData.append('GrupoEquipoId', id.toString());
      });
      
      formData.append('FechaPrestamoEsperada', carrito[grupoid[0]].fecha_inicio || '');
      formData.append('FechaDevolucionEsperada', carrito[grupoid[0]].fecha_final || '');
      formData.append('CarnetUsuario', carnet);
      formData.append('Observacion', '');


      if (contrato) {
          formData.append('Contrato', contrato, 'contrato.html');
      }
        
      return this.http.post(this.url, formData);
    
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


  obtenerPrestamosPorUsuario(carnet: string , estadoPrestamo: string) {
    const APIurl = `${this.url}/historial?carnetUsuario=${carnet}&estadoPrestamo=${estadoPrestamo}`;
    return this.http.get<any[]>(APIurl).pipe(
       map(data =>{ 

         if (!data || !Array.isArray(data)) {
         return [];
       }
        
        return data.map(item => this.mapearPrestamo(item));
  })
);

  }

  obtenercontratoPrestamo(id : number){
    const APIurl = `${this.url}/contrato/${id}`;

    return this.http.get<string[]>(APIurl).pipe(
      map(response => {
        const base64String = response[0];

        const htmlContent = decodeURIComponent(escape(atob(base64String)));

        return htmlContent;

      })

    )

  }




}
