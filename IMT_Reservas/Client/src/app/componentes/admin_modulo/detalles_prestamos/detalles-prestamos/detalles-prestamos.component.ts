import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-detalles-prestamos',
  imports: [],
  templateUrl: './detalles-prestamos.component.html',
  styleUrl: './detalles-prestamos.component.css'
})
export class DetallesPrestamosComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/detalles_prestamos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/detalles_prestamos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/detalles_prestamos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/detalles_prestamos/eliminar']);
  }
}
