import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-muebles',
  standalone: true,
  imports: [],
  templateUrl: './muebles.component.html',
  styleUrl: './muebles.component.css'
})
export class MueblesComponent {
constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/muebles/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/muebles/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/muebles/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/muebles/eliminar']);
  }
}
