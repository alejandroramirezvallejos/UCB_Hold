import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map, throwError } from 'rxjs';
import { Carrito } from '../../../models/carrito';
import { PrestamoDto } from '../../../models/admin/Prestamos';
@Injectable({
  providedIn: 'root',
})
export class PrestamosAPIService {
  private url = environment.apiUrl + '/api/Prestamo';
  constructor(private http: HttpClient) {}
  private mapearPrestamo(item: any): PrestamoDto {
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
    } as PrestamoDto;
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
    const firstItem = Object.values(carrito).find(
      (item) => item && item.cantidad > 0,
    );
    if (!firstItem?.fecha_inicio || !firstItem?.fecha_final) {
      return throwError(
        () => new Error('Fechas de préstamo incompletas en el carrito'),
      );
    }

    const fechaInicio = new Date(`${firstItem.fecha_inicio}T00:00:00`);
    const fechaFinal = new Date(`${firstItem.fecha_final}T00:00:00`);
    if (Number.isNaN(fechaInicio.getTime()) || Number.isNaN(fechaFinal.getTime())) {
      return throwError(
        () => new Error('Formato de fechas inválido en el carrito'),
      );
    }
    if (fechaInicio >= fechaFinal) {
      return throwError(
        () => new Error('La fecha de inicio debe ser menor a la fecha final'),
      );
    }
    const envio = {
      CarnetUsuario: carnet,
      FechaPrestamoEsperada: firstItem.fecha_inicio,
      FechaDevolucionEsperada: firstItem.fecha_final,
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
    const APIurl = `${this.url}/${Id}/estado`;
    const envio = {
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
    const APIurl = `${environment.apiUrl}/api/Contrato/${id}`;
    return this.http.get<any>(APIurl).pipe(
      map((response) => {
        const wrapped = response?.Value ?? response?.value ?? response;
        let base64String = '';

        if (Array.isArray(wrapped)) {
          base64String = wrapped[0] || '';
        } else if (typeof wrapped === 'string') {
          base64String = wrapped;
        } else if (wrapped && typeof wrapped === 'object') {
          base64String = wrapped.ContenidoBase64 || wrapped.contenidoBase64 || '';
        }

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
