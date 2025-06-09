import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IniciarSesionService {
   private apiUrl = environment.apiUrl + '/api/Usuario/iniciarSesion';
  constructor(private http : HttpClient) { }

iniciarsesion(correo: string, contraseña: string) {
  const api = `${this.apiUrl}?email=${correo}&contrasena=${contraseña}`;
  return this.http.get<any>(api).pipe(
    map(item=>({
      id : item.carnet,
      carnet: item.carnet,
      nombre: item.nombre,
      apellido_materno: item.apellidoMaterno,
      apellido_paterno: item.apellidoPaterno,
      rol: item.rol,
      carrera: item.carreraNombre,
      correo: item.email,
      telefono: item.telefono,
      nombre_referencia: item.nombreReferencia,
      telefono_referencia: item.telefonoReferencia,
      email_referencia: item.emailReferencia
    })));
}

}
