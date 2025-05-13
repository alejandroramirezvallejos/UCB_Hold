import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-prestamos',
  standalone: true,
  imports: [],
  templateUrl: './prestamos.component.html',
  styleUrl: './prestamos.component.css'
})
export class PrestamosComponent {
constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/prestamos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/prestamos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/prestamos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/prestamos/eliminar']);
  }
}
