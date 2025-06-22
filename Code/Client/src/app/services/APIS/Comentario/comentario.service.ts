import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ComentarioService {
  apiUrl = environment.apiUrl + '/api/Comentario';
  constructor(private http : HttpClient) { }

  obtenerComentariosPorGrupoEquipo(idGrupoEquipo: string) {
    const url = `${this.apiUrl}/grupo/${idGrupoEquipo}`;
    return this.http.get<any[]>(url).pipe(
      map(data => data.map(item => ({
        Id: item.Id,
        CarnetUsuario: item.CarnetUsuario,
        NombreUsuario: item.NombreUsuario,
        ApellidoPaternoUsuario: item.ApellidoPaternoUsuario,
        IdGrupoEquipo: item.IdGrupoEquipo,
        Contenido: item.Contenido,
        Likes: item.Likes,
        FechaCreacion: item.FechaCreacion
      })))
      ); 
  }

  crearComentario(carnetUsuario: string, idGrupoEquipo: string, contenido: string) {

    const envio = {
      CarnetUsuario: carnetUsuario,
      IdGrupoEquipo: idGrupoEquipo,
      Contenido: contenido
    };

    return this.http.post<any>(this.apiUrl, envio);
  }

  eliminarComentario(idComentario: string) {
    const url = `${this.apiUrl}/${idComentario}`;
    return this.http.delete(url);
  }

  aumentarlikeComentario(idComentario: string) {
    const url = `${this.apiUrl}/${idComentario}/like`;

    return this.http.post(url,null);

  }



}
