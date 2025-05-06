import { Injectable } from '@angular/core';
import { Carrito } from '../../../models/carrito';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Usuario } from '../../../models/usuario';
@Injectable({
  providedIn: 'root'
})
export class CarritoService {

  private apiurl = environment.apiUrl + '/EnviarCarrito';
  constructor(private http : HttpClient) { }

  ReservarCarrito(item: Carrito , usuario : Usuario) {

    return this.http.post<Carrito>(this.apiurl, item).pipe(
      map((response: Carrito) => {
        return response;
      })
    );


  }



  
}
