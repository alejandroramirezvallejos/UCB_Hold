import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs';
import { Carrito } from '../../../models/carrito';
import { Prestamos } from '../../../models/admin/Prestamos';
@Injectable({
  providedIn: 'root',
})
export class PrestamosAPIService {
  private url = environment.apiUrl + '/api/Prestamo';
  constructor(private http: HttpClient) {}
  private mapearPrestamo(item: any): Prestamos {
    return {
      Id: item.Id,
      CarnetUsuario: item.CarnetUsuario,
      NombreUsuario: item.NombreUsuario,
      ApellidoPaternoUsuario: item.ApellidoPaternoUsuario,
      TelefonoUsuario: item.TelefonoUsuario,
      NombreGrupoEquipo: item.NombreGrupoEquipo,
      CodigoImt: item.CodigoImt,
      FechaSolicitud: item.FechaSolicitud
        ? new Date(item.FechaSolicitud)
        : null,
      FechaPrestamoEsperada: item.FechaPrestamoEsperada
        ? new Date(item.FechaPrestamoEsperada)
        : null,
      FechaPrestamo: item.FechaPrestamo ? new Date(item.FechaPrestamo) : null,
      FechaDevolucionEsperada: item.FechaDevolucionEsperada
        ? new Date(item.FechaDevolucionEsperada)
        : null,
      FechaDevolucion: item.FechaDevolucion
        ? new Date(item.FechaDevolucion)
        : null,
      Observacion: item.Observacion,
      EstadoPrestamo: item.EstadoPrestamo,
      IdContrato: item.IdContrato,
      FileId: item.FileId,
      Ubicacion_Equipo: item.Ubicacion_Equipo,
      Nombre_Gavetero: item.Nombre_Gavetero,
      Nombre_Mueble: item.Nombre_Mueble,
      Ubicacion_Mueble: item.Ubicacion_Mueble,
    } as Prestamos;
  }
  obtenerPrestamos() {
    return this.http
      .get<any>(this.url)
      .pipe(
        map((data: any) =>
          (data.Value || data).map((item: any) => this.mapearPrestamo(item)),
        ),
      );
  }
  crearPrestamo(carrito: Carrito, carnet: string, contrato: Blob | null) {
    const grupoid: number[] = [];
    for (const [key, value] of Object.entries(carrito)) {
      if (carrito[Number(key)].cantidad > 0) {
        for (let i = 0; i < carrito[Number(key)].cantidad; i++) {
          grupoid.push(Number(key));
        }
      }
    }
    const envio = {
      CarnetUsuario: carnet,
      FechaPrestamoEsperada: carrito[grupoid[0]].fecha_inicio || '',
      FechaDevolucionEsperada: carrito[grupoid[0]].fecha_final || '',
      Observacion: '',
    };
    return this.http.post<any>(this.url, envio);
  }
  guardarContratoPrestamo(prestamoId: number, contrato: Blob) {
    const formData = new FormData();
    formData.append('Contrato', contrato, 'contrato.html');
    const url = `${this.url}/${prestamoId}/contrato`;
    return this.http.post(url, formData);
  }
  eliminarPrestamo(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }
  cambiarEstadoPrestamo(Id: number, estado: string) {
    const APIurl = `${this.url}/estadoPrestamo`;
    const envio = {
      Id: Id,
      EstadoPrestamo: estado,
    };
    return this.http.put(APIurl, envio);
  }
  obtenerPrestamosPorUsuario(carnet: string, estadoPrestamo: string) {
    const APIurl = `${this.url}/historial?carnetUsuario=${carnet}&estadoPrestamo=${estadoPrestamo}`;
    return this.http.get<any>(APIurl).pipe(
      map((data: any) => {
        const list = data.Value || data;
        if (!list || !Array.isArray(list)) {
          return [];
        }
        return list.map((item: any) => this.mapearPrestamo(item));
      }),
    );
  }
  obtenercontratoPrestamo(id: number) {
    const APIurl = `${this.url}/contrato/${id}`;
    return this.http.get<string[]>(APIurl).pipe(
      map((response) => {
        if (!response || !Array.isArray(response) || response.length === 0)
          return '';
        const base64String = response[0] || '';
        if (!base64String) return '';
        try {
          const raw = atob(base64String);
          const htmlContent = decodeURIComponent(escape(raw));
          const cleaned = htmlContent
            .replace(/undefined(,\s*undefined)?/g, '')
            .replace(/\[\[\s*\w+\s*\]\]/g, '');
          return cleaned;
        } catch (e) {
          console.error('Error decoding contrato base64', e);
          return '';
        }
      }),
    );
  }
}
