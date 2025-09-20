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
  productos: GrupoEquipo[][] = [];

  cantidadObjetos: number = 20;

  paginaActual: number = 0;


  constructor(private servicio: GrupoequipoService) { };

  ngOnInit(): void {
    this.servicio.getGrupoEquipo(this.categoria , this.producto).subscribe({
      next: (data) =>{
        this.productos = this.paginar(data);
      },
      error: (error) => console.error('Error en componente:', error)
    });

    
  }

  paginar(productos: GrupoEquipo[]): GrupoEquipo[][] {
    const resultado: GrupoEquipo[][] = [];
    for (let i = 0; i < productos.length; i += this.cantidadObjetos) {
      resultado.push(productos.slice(i, i + this.cantidadObjetos));
    }
    return resultado;
  }

  actualizarPagina(pagina: number): void {
    this.paginaActual = pagina;
  }



}
