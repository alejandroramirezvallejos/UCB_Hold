import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-carreras',
  standalone: true,
  imports: [],
  templateUrl: './carreras.component.html',
  styleUrl: './carreras.component.css'
})

export class CarrerasComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/carreras/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/carreras/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/carreras/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/carreras/eliminar']);
  }
}
