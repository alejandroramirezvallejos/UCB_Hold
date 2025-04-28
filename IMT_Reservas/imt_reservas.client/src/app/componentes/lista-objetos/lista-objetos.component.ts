import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GrupoEquipoService } from '../../services/grupo_equipo/grupo-equipo.service';


@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent {

  productos: any[] = [];
  dataa: any = ""
  constructor(private servicio: GrupoEquipoService) { }

  ngOnInit(): void {
    // Llama al servicio y suscríbete al Observable
    this.servicio.getGrupoEquipo().subscribe({
      next: (data) => {
        console.log('Datos recibidos:', data); // 👈 Verifica aquí
        this.productos = data;
        this.dataa = data;
      },
      error: (error) => {
        console.error('Error al obtener los datos del grupo de equipo', error);
      }
    });
  }

  handleImageError(item: any) {
    item.link = null; // Elimina la imagen fallida
  }
}
