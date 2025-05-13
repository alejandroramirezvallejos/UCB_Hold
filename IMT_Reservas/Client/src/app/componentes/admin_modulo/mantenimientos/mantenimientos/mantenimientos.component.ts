import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-mantenimientos',
  standalone: true,
  imports: [],
  templateUrl: './mantenimientos.component.html',
  styleUrl: './mantenimientos.component.css'
})
export class MantenimientosComponent {
constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/mantenimientos/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/mantenimientos/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/mantenimientos/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/mantenimientos/eliminar']);
  }
}
