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

  crearMantenimiento(mantenimiento: any) {
    const envio = {
      FechaMantenimiento: mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: mantenimiento.NombreEmpresaMantenimiento,
      Costo: mantenimiento.Costo,
      DescripcionMantenimiento: mantenimiento.DescripcionMantenimiento,
      CodigoIMT: mantenimiento.CodigoIMT,
      TipoMantenimiento: mantenimiento.TipoMantenimiento,
      DescripcionEquipo: mantenimiento.DescripcionEquipo
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
        CodigoImtEquipo: item.CodigoIMT,
        DescripcionEquipo: item.DescripcionEquipo
      })))
    );

  }
  //NO ESTA EN SWAGGER
  editarMantenimiento(mantenimiento: any) {
    const envio = {
      Id: mantenimiento.Id,
      FechaMantenimiento: mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: mantenimiento.NombreEmpresaMantenimiento,
      Costo: mantenimiento.Costo,
      DescripcionMantenimiento: mantenimiento.DescripcionMantenimiento,
      CodigoIMT: mantenimiento.CodigoIMT,
      TipoMantenimiento: mantenimiento.TipoMantenimiento,
      DescripcionEquipo: mantenimiento.DescripcionEquipo
    };

    return this.http.put<any>(`${this.apiUrl}/${mantenimiento.Id}`, envio);
  }


  eliminarMantenimiento(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

}
