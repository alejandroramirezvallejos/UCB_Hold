import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-grupos-equipos',
  standalone: true,
  imports: [],
  templateUrl: './grupos-equipos.component.html',
  styleUrl: './grupos-equipos.component.css'
})
export class GruposEquiposComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/grupos_equipos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/grupos_equipos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/grupos_equipos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/grupos_equipos/eliminar']);
  }
}
