import { Component } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-administrador',
  standalone: true,
  imports: [],
  templateUrl: './administrador.component.html',
  styleUrls: ['./administrador.component.css']
})
export class AdministradorComponent {
  constructor(public  router: Router) {
  }

  irAccesorios(){
    this.router.navigate(['/admin/accesorios']);
  }

  irCarreras(){
    this.router.navigate(['/admin/carreras']);
  }
  irUsuarios(){
    this.router.navigate(['/admin/usuarios']);
  }
  irCategorias(){
    this.router.navigate(['/admin/categorias']);
  }
  irComponentes(){
    this.router.navigate(['/admin/componentes']);
  }
  irDetallesPrestamos(){
    this.router.navigate(['/admin/detalles_prestamos']);
  }
  irDetallesMantenimientos(){
    this.router.navigate(['/admin/detalles_mantenimientos']);
  }
  irEmpresasMantenimiento(){
    this.router.navigate(['/admin/empresas_mantenimiento']);
  }
  irEquipos(){
    this.router.navigate(['/admin/equipos']);
  }
  irGaveteros(){
    this.router.navigate(['/admin/gaveteros']);
  }
  irGruposEquipos(){
    this.router.navigate(['/admin/grupos_equipos']);
  }
  irMantenimientos(){
    this.router.navigate(['/admin/mantenimientos']);
  }
  irMuebles(){
    this.router.navigate(['/admin/muebles']);
  }
  irPrestamos(){
    this.router.navigate(['/admin/prestamos']);
  }
}
