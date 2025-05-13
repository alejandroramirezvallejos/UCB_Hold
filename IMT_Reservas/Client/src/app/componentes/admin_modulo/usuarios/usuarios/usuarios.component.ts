import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [],
  templateUrl: './usuarios.component.html',
  styleUrl: './usuarios.component.css'
})
export class UsuariosComponent {
constructor(private route: Router) { }
  irCrear() {
    this.route.navigate(['/admin/usuarios/crear']);
  }
  irObtener() {
    this.route.navigate(['/admin/usuarios/obtener']);
  }
  irActualizar() {
    this.route.navigate(['/admin/usuarios/actualizar']);
  }
  irEliminar() {
    this.route.navigate(['/admin/usuarios/eliminar']);
  }
}
