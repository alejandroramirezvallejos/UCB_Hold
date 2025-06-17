import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { EmpresaMantenimiento } from '../../../models/admin/EmpresaMantenimiento';
import { map } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class EmpresamantenimientoService {
  private apiUrl = environment.apiUrl + '/api/EmpresaMantenimiento';
  constructor(private http: HttpClient) { }

  crearEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    const envio = {
      NombreEmpresa: empresa.NombreEmpresa,
      NombreResponsable: empresa.NombreResponsable,
      ApellidoResponsable: empresa.ApellidoResponsable,
      Telefono: empresa.Telefono,
      Nit: empresa.Nit,
      Direccion: empresa.Direccion
    };

    return this.http.post<any>(this.apiUrl, envio);

  }


  obtenerEmpresaMantenimiento(){

    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
        Id : item.Id,
        NombreEmpresa: item.NombreEmpresa,
        NombreResponsable: item.NombreResponsable,
        ApellidoResponsable: item.ApellidoResponsable,
        Telefono: item.Telefono,
        Nit: item.Nit,
        Direccion: item.Direccion
      })))
    );

  }

  actualizarEmpresaMantenimiento(empresa: EmpresaMantenimiento) {
    const envio = {
      Id: empresa.Id,
      NombreEmpresa: empresa.NombreEmpresa,
      NombreResponsable: empresa.NombreResponsable,
      ApellidoResponsable: empresa.ApellidoResponsable,
      Telefono: empresa.Telefono,
      Nit: empresa.Nit,
      Direccion: empresa.Direccion
    };

    return this.http.put<any>(this.apiUrl, envio);
  }

  eliminarEmpresaMantenimiento(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }


}
