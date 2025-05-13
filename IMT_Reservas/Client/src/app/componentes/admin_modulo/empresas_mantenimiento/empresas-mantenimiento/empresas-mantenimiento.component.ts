import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-empresas-mantenimiento',
  standalone: true,
  imports: [],
  templateUrl: './empresas-mantenimiento.component.html',
  styleUrl: './empresas-mantenimiento.component.css'
})
export class EmpresasMantenimientoComponent {
  constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/empresas_mantenimiento/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/empresas_mantenimiento/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/empresas_mantenimiento/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/empresas_mantenimiento/eliminar']);
  }
}
