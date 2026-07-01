import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Usuario } from '@entities/user';
import { Carrera } from '@entities/admin';
import { UsuariosCrearComponent } from '../usuarios-crear/usuarios-crear.component';
import { UsuariosEditarComponent } from '../usuarios-editar/usuarios-editar.component';
import { UsuarioServiceAPI } from '@entities/user';
import { CarreraService } from '@entities/career';
import { AvisoEliminarComponent } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { Tabla } from '@shared/lib/admin-table';
import { BuscadorComponent } from '@features/admin-search';
import { extractErrorMessage } from '@shared/lib/error';
import { PrestamosInlineComponent } from '@widgets/admin-inline';
import { AuditPanelComponent } from '@widgets/audit-panel';
import { StickyScrollDirective } from '@shared/lib/directives';
@Component({
  selector: 'app-usuarios-tabla',
  standalone: true,
  imports: [
    StickyScrollDirective,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UsuariosCrearComponent,
    UsuariosEditarComponent,
    AvisoEliminarComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
    BuscadorComponent,
    PrestamosInlineComponent,
    AuditPanelComponent,
  ],
  templateUrl: './usuarios-tabla.component.html',
  styleUrls: ['./usuarios-tabla.component.css'],
})
export class UsuariosTablaComponent extends Tabla implements OnInit {
  expandedRowId: number | null = null;
  auditRefresh = 0;
  expandedCarnet: string | null = null;

  toggleExpandCarnet(carnet: string) {
    this.expandedCarnet = this.expandedCarnet === carnet ? null : carnet;
  }

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }
  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  valoreliminar: number = 0;
  usuarios: Usuario[] = [];
  usuarioscopia: Usuario[] = [];
  carreras: string[] = [];
  usuarioSeleccionado: Usuario = new Usuario();
  override columnas: string[] = [
    'Carnet',
    'Nombre',
    'Apellido Paterno',
    'Apellido Materno',
    'Correo',
    'Teléfono',
    'Rol',
    'Carrera',
    'Referencia',
    'Tel. Referencia',
  ];
  constructor(
    private readonly usuarioapi: UsuarioServiceAPI,
    private carrerasAPI: CarreraService,
  ) {
    super();
  }
  ngOnInit() {
    this.cargarUsuarios();
    this.cargarCarreras();
  }
  crearusuario() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarCarreras() {
    this.carrerasAPI.obtenerCarreras().subscribe({
      next: (data: Carrera[]) => {
        this.carreras = data.map((carrera) => carrera.Nombre ?? '');
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar las carreras, intente más tarde.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  cargarUsuarios() {
    this.usuarioapi.obtenerUsuarios().subscribe({
      next: (data: Usuario[]) => {
        this.usuarios = data;
        this.usuarioscopia = [...this.usuarios];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar los usuarios, intente más tarde.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  actualizarTabla() {
    this.cargarUsuarios();
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.usuarios = this.usuarioscopia.filter((usuario) => {
        switch (event[1]) {
          case 'Carnet':
            return this.normalizeText(usuario.carnet || '').includes(
              busquedaNormalizada,
            );
          case 'Nombre':
            return this.normalizeText(usuario.nombre || '').includes(
              busquedaNormalizada,
            );
          case 'Apellido Paterno':
            return this.normalizeText(usuario.apellido_paterno || '').includes(
              busquedaNormalizada,
            );
          case 'Apellido Materno':
            return this.normalizeText(usuario.apellido_materno || '').includes(
              busquedaNormalizada,
            );
          case 'Correo':
            return this.normalizeText(usuario.correo || '').includes(
              busquedaNormalizada,
            );
          case 'Teléfono':
            return this.normalizeText(usuario.telefono || '').includes(
              busquedaNormalizada,
            );
          case 'Rol':
            return this.normalizeText(usuario.rol || '').includes(
              busquedaNormalizada,
            );
          case 'Carrera':
            return this.normalizeText(usuario.carrera || '').includes(
              busquedaNormalizada,
            );
          case 'Referencia':
            return this.normalizeText(usuario.nombre_referencia || '').includes(
              busquedaNormalizada,
            );
          case 'Tel. Referencia':
            return this.normalizeText(
              usuario.telefono_referencia || '',
            ).includes(busquedaNormalizada);
          default:
            return (
              this.normalizeText(usuario.carnet || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.nombre || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.apellido_paterno || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.apellido_materno || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.correo || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.telefono || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.rol || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.carrera || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.nombre_referencia || '').includes(
                busquedaNormalizada,
              ) ||
              this.normalizeText(usuario.telefono_referencia || '').includes(
                busquedaNormalizada,
              )
            );
        }
      });
    } else {
      this.usuarios = [...this.usuarioscopia];
    }
  }
  limpiarBusqueda() {
    this.usuarios = [...this.usuarioscopia];
  }
  editarUsuario(usuario: Usuario) {
    this.botoncrear.set(false);
    this.usuarioSeleccionado = usuario;
    this.botoneditar.set(true);
  }
  eliminarUsuario(i: number) {
    this.valoreliminar = i;
    this.alertaeliminar = true;
  }
  confirmarEliminacion() {
    const usuarioAEliminar = this.usuarios[this.valoreliminar];
    this.usuarioapi.eliminarUsuario(usuarioAEliminar.id || '').subscribe({
      next: (response) => {
        this.mensajeexito = 'Usuario eliminado exitosamente.';
        this.exito.set(true);
        this.auditRefresh++;
        this.usuarios.splice(this.valoreliminar, 1);
        this.usuarioscopia = [...this.usuarios];
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al eliminar el usuario, intente más tarde.',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      },
    });
  }
  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.valoreliminar = 0;
  }

  override sortTable(e: { col: string; dir: 'asc' | 'desc' }) {
    const m: Record<string, keyof Usuario> = {
      Carnet: 'carnet',
      Nombre: 'nombre',
      'Apellido Paterno': 'apellido_paterno',
      'Apellido Materno': 'apellido_materno',
      Correo: 'correo',
      Teléfono: 'telefono',
      Rol: 'rol',
      Carrera: 'carrera',
    };
    const k = m[e.col];
    if (!k) return;
    this.usuarios = [...this.usuarios].sort((a, b) => {
      const va = this.sortableValue(a, k);
      const vb = this.sortableValue(b, k);
      return e.dir === 'asc' ? va.localeCompare(vb) : vb.localeCompare(va);
    });
  }
}
