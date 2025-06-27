import { UsuarioService } from './../../services/usuario/usuario.service';
import { GrupoequipoService } from './../../services/APIS/GrupoEquipo/grupoequipo.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-favoritos',
  imports: [],
  templateUrl: './favoritos.component.html',
  styleUrl: './favoritos.component.css'
})
export class FavoritosComponent {

  favoritos : string[] = [];

  constructor(private grupoequipo : GrupoequipoService , private usuario : UsuarioService){

  }

  ngOnInit() {
    this.obtenerFavoritos();
  }

  obtenerFavoritos() {
    this.grupoequipo.obtenerFavoritosPorCarnetUsuario(this.usuario.obtenercarnet()).subscribe(
      (data: any[]) => {
        for (let item of data) {
          this.favoritos.push("dsadsd");
        }
      }
      , (error: any) => {
        console.error('Error al obtener los favoritos:', error);
      }
    );

  }


}
