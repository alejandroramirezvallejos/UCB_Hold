import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';
import { Carrito } from '../../../models/carrito';
import { PrestamoDto } from '../../../models/admin/Prestamos';
@Injectable({
  providedIn: 'root'
})
export class PrestamosAPIService {
  private url = environment.apiUrl + '/api/Prestamo';
  constructor(private http: HttpClient) { }
  private mapearPrestamo(item: any): PrestamoDto {
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
      Ubicacion_Equipo: item.UbicacionEquipo || item.Ubicacion_Equipo,
      Nombre_Gavetero: item.NombreGavetero || item.Nombre_Gavetero,
      Nombre_Mueble: item.NombreMueble || item.Nombre_Mueble,
      Ubicacion_Mueble: item.UbicacionMueble || item.Ubicacion_Mueble
    } as PrestamoDto;
  }


  obtenerPrestamos() {
    return this.http.get<any>(this.url).pipe(
      map(data => {
        const arrayData = Array.isArray(data) ? data : (data?.Value || data?.value || []);
        return arrayData.map((item: any) => this.mapearPrestamo(item));
      })
    );
  }




  crearPrestamo(carrito: Carrito, carnet: string, contrato: (string | null)) {
    const grupoid: number[] = [];
    for (const [key, value] of Object.entries(carrito)) {
      if (carrito[Number(key)].cantidad > 0) {
        for (let i = 0; i < carrito[Number(key)].cantidad; i++) {
          grupoid.push(Number(key));
        }
      }
    }
    const body = {
      GrupoEquipoId: grupoid,
      FechaPrestamoEsperada: carrito[grupoid[0]].fecha_inicio || null,
      FechaDevolucionEsperada: carrito[grupoid[0]].fecha_final || null,
      CarnetUsuario: carnet,
      Observacion: '',
      Contrato: contrato || null
    };

    return this.http.post(this.url, body);
  }



  eliminarPrestamo(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }



  cambiarEstadoPrestamo(Id: number, estado: string, observacion?: string) {
    const APIurl = `${this.url}/${Id}/estado`;
    const envio: any = {
      EstadoPrestamo: estado
    };
    if (observacion !== undefined && observacion !== null) {
      envio.Observacion = observacion;
    }
    return this.http.put(APIurl, envio)
  }


  obtenerPrestamosPorUsuario(carnet: string, estadoPrestamo: string) {
    const APIurl = `${this.url}/historial?carnetUsuario=${carnet}&estadoPrestamo=${estadoPrestamo}`;
    return this.http.get<any>(APIurl).pipe(
      map(data => {
        const arrayData = Array.isArray(data) ? data : (data?.Value || data?.value || []);
        if (!Array.isArray(arrayData)) {
          return [];
        }
        return arrayData.map((item: any) => this.mapearPrestamo(item));
      })
    );
  }



  obtenercontratoPrestamo(id: number) {
    const APIurl = `${this.url}/contrato/${id}`;
    return this.http.get<{ contrato: string }>(APIurl).pipe(
      map(response => {
        if (!response || !response.contrato) return '';
        return response.contrato;
      })
    )
  }
}
