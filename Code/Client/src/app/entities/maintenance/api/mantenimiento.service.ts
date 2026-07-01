import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Mantenimientos } from '@entities/admin';
import { ApiResponse, extractApiValue } from '@shared/api';
import { MantenimientoApiItem } from './mantenimiento-api-item';
import { MantenimientoCreationForm } from '../model/mantenimiento-creation-form';
@Injectable({
  providedIn: 'root',
})
export class MantenimientoService {
  private readonly apiUrl = environment.apiUrl + '/api/Mantenimiento';
  constructor(private readonly http: HttpClient) {}

  crearMantenimiento(
    mantenimiento: MantenimientoCreationForm,
    equipos: Map<
      number,
      { TipoMantenimiento: string; DescripcionEquipo: string; nombre: string }
    >,
  ) {
    const codigosimt: number[] = [];
    const tiposmantenimiento: string[] = [];
    const descripcionequipos: string[] = [];
    equipos.forEach((value, key) => {
      codigosimt.push(key);
      tiposmantenimiento.push(value.TipoMantenimiento);
      descripcionequipos.push(value.DescripcionEquipo);
    });
    const envio = {
      FechaMantenimiento: mantenimiento.FechaMantenimiento,
      FechaFinalMantenimiento: mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: mantenimiento.NombreEmpresaMantenimiento,
      Costo: mantenimiento.Costo,
      Descripcion: mantenimiento.DescripcionMantenimiento,
      CodigoIMT: codigosimt,
      TiposMantenimiento: tiposmantenimiento,
      DescripcionesEquipo: descripcionequipos,
    };
    return this.http.post<unknown>(this.apiUrl, envio);
  }

  obtenerMantenimientos(): Observable<Mantenimientos[]> {
    return this.http.get<ApiResponse<MantenimientoApiItem[]>>(this.apiUrl).pipe(
      map((data) =>
        extractApiValue(data, []).map((item) => ({
          Id: item.Id,
          IdEmpresa: null,
          NombreEmpresaMantenimiento: item.NombreEmpresaMantenimiento,
          FechaMantenimiento: item.FechaMantenimiento
            ? new Date(item.FechaMantenimiento)
            : null,
          FechaFinalDeMantenimiento: item.FechaFinalMantenimiento
            ? new Date(item.FechaFinalMantenimiento)
            : null,
          Costo: item.Costo,
          Descripcion: item.Descripcion,
          TipoMantenimiento: item.TipoMantenimiento,
          NombreGrupoEquipo: item.NombreGrupoEquipo,
          CodigoImtEquipo: item.CodigoImtEquipo,
          DescripcionEquipo: item.DescripcionEquipo,
        })),
      ),
    );
  }

  eliminarMantenimiento(id: number) {
    return this.http.delete<unknown>(`${this.apiUrl}/${id}`);
  }
}
