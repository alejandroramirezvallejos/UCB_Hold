import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-equipos',
  standalone: true,
  imports: [],
  templateUrl: './equipos.component.html',
  styleUrl: './equipos.component.css'
})
export class EquiposComponent {
constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/equipos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/equipos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/equipos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/equipos/eliminar']);
  }
}
