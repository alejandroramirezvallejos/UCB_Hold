import { Component } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-componentes',
  standalone: true,
  imports: [],
  templateUrl: './componentes.component.html',
  styleUrl: './componentes.component.css'
})
export class ComponentesComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/componentes/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/componentes/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/componentes/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/componentes/eliminar']);
  }
}
