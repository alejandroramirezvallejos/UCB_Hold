import { Component } from '@angular/core';
import {Router} from '@angular/router';
@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [],
  templateUrl: './categorias.component.html',
  styleUrl: './categorias.component.css'
})
export class CategoriasComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/categorias/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/categorias/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/categorias/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/categorias/eliminar']);
  }
}
