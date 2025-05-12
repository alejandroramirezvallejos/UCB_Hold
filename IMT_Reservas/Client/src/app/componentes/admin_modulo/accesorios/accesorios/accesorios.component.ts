import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-accesorios',
  standalone: true,
  imports: [],
  templateUrl: './accesorios.component.html',
  styleUrl: './accesorios.component.css'
})
export class AccesoriosComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/accesorios/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/accesorios/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/accesorios/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/accesorios/eliminar']);
  }
}
