import { Component, createNgModuleRef } from '@angular/core';
import {Router} from '@angular/router';
import { SidebardComponent } from '../../sidebard/sidebard.component';
import { AccesoriosTablaComponent } from '../tablas/accesorios/accesorios-tabla/accesorios-tabla.component';
import { CarrerasTablaComponent } from '../tablas/carreras/carreras-tabla/carreras-tabla.component';
import { CategoriasTablaComponent } from '../tablas/categorias/categorias-tabla/categorias-tabla.component';
@Component({
  selector: 'app-administrador',
  standalone: true,
  imports: [SidebardComponent,AccesoriosTablaComponent,CarrerasTablaComponent,CategoriasTablaComponent],
  templateUrl: './administrador.component.html',
  styleUrls: ['./administrador.component.css']
})
//TODO : Agregar los componentes de las tablas
export class AdministradorComponent {
  tablas : string[] = ['Accesorios', 'Carreras', 'Usuarios', 'Categorias', 'Componentes', "Empresas de Mantenimiento", 'Equipos', 'Gaveteros', 'Grupos de Equipos', 'Mantenimientos', 'Muebles', 'Prestamos', "Equipos que Necesitan Mantenimiento", "Ubicacion de Grupos de Equipos"];
  
  item : string = 'Accesorios';

  constructor(public  router: Router) {
  }

  clickitem(item : string){
    this.item = item;
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
