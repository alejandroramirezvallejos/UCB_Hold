import { Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { Router } from '@angular/router';
@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent {
  @Input() categoria: string = ''; 
  @Input() producto: string = '';
  productos: GrupoEquipo[] = [];

  constructor(private servicio: GrupoequipoService) { };

  ngOnInit(): void {
    this.servicio.getGrupoEquipo(this.categoria , this.producto).subscribe({
      next: (data) => this.productos = data,
      error: (error) => console.error('Error en componente:', error)
    });

    console.log(this.productos)
  }





}
