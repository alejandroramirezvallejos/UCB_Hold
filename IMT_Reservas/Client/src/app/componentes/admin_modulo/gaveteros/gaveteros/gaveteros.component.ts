import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-gaveteros',
  standalone: true,
  imports: [],
  templateUrl: './gaveteros.component.html',
  styleUrl: './gaveteros.component.css'
})
export class GaveterosComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/gaveteros/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/gaveteros/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/gaveteros/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/gaveteros/eliminar']);
  }
}
