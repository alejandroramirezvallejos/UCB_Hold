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
      id : item.Carnet,
      carnet: item.Carnet,
      nombre: item.Nombre,
      apellido_materno: item.ApellidoMaterno,
      apellido_paterno: item.ApellidoPaterno,
      rol: item.Rol,
      carrera: item.CarreraNombre,
      correo: item.Email,
      telefono: item.Telefono,
      nombre_referencia: item.NombreReferencia,
      telefono_referencia: item.TelefonoReferencia,
      email_referencia: item.EmailReferencia
    })));
}

}
