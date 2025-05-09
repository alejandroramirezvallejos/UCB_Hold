import { Injectable } from '@angular/core';
import { Carrito } from '../../../models/carrito';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class MandarcarritoService {

  private apiurl = environment.apiUrl + '/api/Prestamo';
  constructor(private http : HttpClient) { }

  ReservarCarrito(item: Carrito, usuarioid: number) {
    const data = { ...item, usuarioid };
  
    return this.http.post<Carrito>(this.apiurl, data).pipe(
      map((response: Carrito) => {
        return response;
      })
    );
  }
  

}
