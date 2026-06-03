import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuditLogDto } from '../../../models/admin/AuditLog';

@Injectable({ providedIn: 'root' })
export class AuditLogApiService {
  private readonly url = `${environment.apiUrl}/api/AuditLog`;

  constructor(private http: HttpClient) {}

  getAuditLog(
    entidad?: string,
    carnet?: string,
    desde?: string,
    hasta?: string
  ): Observable<AuditLogDto[]> {
    let params = new HttpParams();
    if (entidad) params = params.set('entidad', entidad);
    if (carnet)  params = params.set('carnet', carnet);
    if (desde)   params = params.set('desde', desde);
    if (hasta)   params = params.set('hasta', hasta);

    return this.http.get<any>(this.url, { params }).pipe(
      map(res => {
        const data = res?.Value ?? res?.value ?? res ?? [];
        return (Array.isArray(data) ? data : []).map((item: any) => ({
          Id: item.Id,
          AdminCarnet: item.AdminCarnet,
          AdminNombre: item.AdminNombre,
          Accion: item.Accion,
          Entidad: item.Entidad,
          EntidadId: item.EntidadId,
          Detalle: item.Detalle,
          Timestamp: item.Timestamp ? new Date(item.Timestamp) : undefined
        } as AuditLogDto))
      })
    );
  }
}
