import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-detalles-mantenimientos',
  imports: [],
  templateUrl: './detalles-mantenimientos.component.html',
  styleUrl: './detalles-mantenimientos.component.css'
})
export class DetallesMantenimientosComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/detalles_mantenimientos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/detalles_mantenimientos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/detalles_mantenimientos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/detalles_mantenimientos/eliminar']);
  }
}
