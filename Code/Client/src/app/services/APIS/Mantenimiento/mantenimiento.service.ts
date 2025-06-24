import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MantenimientoService {
  private apiUrl = environment.apiUrl + '/api/Mantenimiento'; 
  constructor(private http: HttpClient) { }

  crearMantenimiento(mantenimiento: any , equipos: Map<number, { TipoMantenimiento: string; DescripcionEquipo: string; nombre: string }> ) {
    var codigosimt: number[] = [];
    var tiposmantenimiento: string[] = [];
    var descripcionequipos: string[] = [];

    equipos.forEach((value, key) => {
      codigosimt.push(key);
      tiposmantenimiento.push(value.TipoMantenimiento);
      descripcionequipos.push(value.DescripcionEquipo);
    });

    const envio = {
      FechaMantenimiento: mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: mantenimiento.NombreEmpresaMantenimiento,
      Costo: mantenimiento.Costo,
      DescripcionMantenimiento: mantenimiento.DescripcionMantenimiento,
      CodigoIMT: codigosimt,
      TipoMantenimiento: tiposmantenimiento,
      DescripcionEquipo: descripcionequipos
    };

    return this.http.post<any>(this.apiUrl, envio);
  }

  obtenerMantenimientos() {
    
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        NombreEmpresaMantenimiento: item.NombreEmpresaMantenimiento,
        FechaMantenimiento: item.FechaMantenimiento,
        FechaFinalDeMantenimiento: item.FechaFinalDeMantenimiento,
        Costo: item.Costo,
        Descripcion: item.Descripcion,
        TipoMantenimiento: item.TipoMantenimiento,
        NombreGrupoEquipo: item.NombreGrupoEquipo,
        CodigoImtEquipo: item.CodigoImtEquipo,
        DescripcionEquipo: item.DescripcionEquipo
      })))
    );

  }
  

  eliminarMantenimiento(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

}
