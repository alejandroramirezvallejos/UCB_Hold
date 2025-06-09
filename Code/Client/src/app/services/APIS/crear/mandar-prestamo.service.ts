import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Carrito } from '../../../models/carrito';

@Injectable({
  providedIn: 'root'
})
export class MandarPrestamoService {
  private apiUrl = environment.apiUrl + '/api/Prestamo';
  constructor(private http : HttpClient ) { }

  crearPrestamo(carrito: Carrito , carnet : string , contrato : (Blob | null)  ) {
    const grupoid : number [] = [];

    for( const [key,value] of Object.entries(carrito)) {
        if(carrito[Number(key)].cantidad>0){
          for(let i = 0 ; i < carrito[Number(key)].cantidad ; i++){
            grupoid.push(Number(key));
          }
        }
    }

    console.log(grupoid); 

    const formulario = {
      grupoEquipoId: grupoid,
      fechaPrestamoEsperada: carrito[grupoid[0]].fecha_inicio,
      fechaDevolucionEsperada: carrito[grupoid[0]].fecha_final,
      carnetUsuario : carnet,
      contrato : null,
      observacion : null,
    };


    return this.http.post(this.apiUrl, formulario);

  }
}
