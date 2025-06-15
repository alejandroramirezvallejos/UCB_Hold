import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class EmpresamantenimientoService {
  private apiUrl = environment.apiUrl + '/api/EmpresaMantenimiento';
  constructor(private http: HttpClient) { }

  obtenerEmpresaMantenimiento(){

    
  }


}
