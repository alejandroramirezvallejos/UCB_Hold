import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SidebarComponent } from '@widgets/admin-sidebar';
import { AccesoriosTablaComponent } from '@features/admin-accessories';
import { CarrerasTablaComponent } from '@features/admin-careers';
import { CategoriasTablaComponent } from '@features/admin-categories';
import { UsuariosTablaComponent } from '@features/admin-users';
import { ComponentesTablaComponent } from '@features/admin-components';
import { EmpresasMantenimientoTablaComponent } from '@features/admin-maintenance-companies';
import { EquiposTablaComponent } from '@features/admin-equipment';
import { GaveterosTablaComponent } from '@features/admin-lockers';
import { GruposEquiposTablaComponent } from '@features/admin-equipment-groups';
import { MantenimientosTablaComponent } from '@features/admin-maintenance';
import { MueblesTablaComponent } from '@features/admin-furniture';
import { PrestamosTablaComponent } from '@features/admin-loans';
import { UsuarioService } from '@entities/user';
@Component({
  selector: 'app-administrador',
  standalone: true,
  imports: [
    SidebarComponent,
    AccesoriosTablaComponent,
    CarrerasTablaComponent,
    UsuariosTablaComponent,
    CategoriasTablaComponent,
    ComponentesTablaComponent,
    EmpresasMantenimientoTablaComponent,
    EquiposTablaComponent,
    GaveterosTablaComponent,
    GruposEquiposTablaComponent,
    MantenimientosTablaComponent,
    MueblesTablaComponent,
    PrestamosTablaComponent,
  ],
  templateUrl: './administrador.component.html',
  styleUrls: ['./administrador.component.css'],
})
export class AdministradorComponent {
  tablas: string[] = [
    'Prestamos',
    'Carreras',
    'Usuarios',
    'Categorias',
    'Componentes',
    'Empresas de Mantenimiento',
    'Equipos',
    'Gaveteros',
    'Grupos de Equipos',
    'Mantenimientos',
    'Muebles',
    'Accesorios',
  ];
  item: string = 'Prestamos';
  constructor(
    public router: Router,
    private usuario: UsuarioService,
  ) {}
  ngOnInit() {
    if (this.usuario.estaVacio()) {
      this.router.navigate(['/Iniciar-Sesion']);
    } else if (this.usuario.obtenerRol() != 'administrador') {
      this.router.navigate(['/home']);
    }
  }
  clickitem(item: string) {
    this.item = item;
  }
}
