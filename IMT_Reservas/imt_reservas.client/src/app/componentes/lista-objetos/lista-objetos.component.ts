import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GrupoEquipoService } from '../../services/grupo_equipo/grupo-equipo.service';
import { GrupoEquipo } from '../../models/grupo_equipo';
import { Router } from '@angular/router';
@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent {

  productos: GrupoEquipo[] = [];

  constructor(private servicio: GrupoEquipoService) { };

  ngOnInit(): void {
    this.servicio.getGrupoEquipo().subscribe({
      next: (data) => this.productos = data,
      error: (error) => console.error('Error en componente:', error)
    });
  }

  handleImageError(item: GrupoEquipo) {
    item.link = undefined; // Elimina la imagen fallida
  }



}
