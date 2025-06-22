import { Component, Input } from '@angular/core';
import { Comentario } from '../../../../models/Comentario';
import { ComentarioService } from '../../../../services/APIS/Comentario/comentario.service';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-comentarios',
  imports: [CommonModule , FormsModule],
  templateUrl: './comentarios.component.html',
  styleUrl: './comentarios.component.css'
})
export class ComentariosComponent {

  @Input() idGrupoEquipo: string = "";
  comentarios : Comentario[] = [];

  nuevocomentario: string = "";

  constructor(private comentarioApi : ComentarioService , public usuario : UsuarioService  ){}; 

  ngOnInit() {
    this.cargarComentarios();
  }

  cargarComentarios() {
    this.comentarioApi.obtenerComentariosPorGrupoEquipo(this.idGrupoEquipo).subscribe({
      next: (data) => {
        this.comentarios = data;
      },
      error: (error) => {
        if(error.error.mensaje==="No se encontraron comentarios para el grupo."){
          return ; 
        }
        alert( error.error.error +": "+error.error.mensaje)
      }
    });
  }

  darlike(indice : number){
    this.comentarioApi.aumentarlikeComentario(this.comentarios[indice].Id).subscribe({
      next: (data) => {
        this.cargarComentarios();
      },
      error: (error) => {
        alert( error.error.error +": "+error.error.mensaje)
      }
    });
  }

  crearComentario() {
    this.comentarioApi.crearComentario(this.usuario.obtenercarnet(), this.idGrupoEquipo, this.nuevocomentario).subscribe({
      next: (data) => {
        this.cargarComentarios();
        this.nuevocomentario = "";
      }
      , error: (error) => {
        alert( error.error.error +": "+error.error.mensaje)
      }
    });
  }

  eliminarComentario(idComentario: string) {
    this.comentarioApi.eliminarComentario(idComentario).subscribe({
      next: (data) => {
        this.cargarComentarios();
      },
      error: (error) => {
        alert( error.error.error +": "+error.error.mensaje)
      }
    });
  }


}
