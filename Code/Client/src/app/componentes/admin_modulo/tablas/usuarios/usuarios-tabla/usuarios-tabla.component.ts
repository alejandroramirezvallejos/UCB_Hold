import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuariosCrearComponent } from '../usuarios-crear/usuarios-crear.component';
import { UsuariosEditarComponent } from '../usuarios-editar/usuarios-editar.component';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-usuarios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, UsuariosCrearComponent, UsuariosEditarComponent,AvisoEliminarComponent, MostrarerrorComponent, AvisoExitoComponent],
  templateUrl: './usuarios-tabla.component.html',
  styleUrls: ['./usuarios-tabla.component.css']
})
export class UsuariosTablaComponent extends BaseTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  valoreliminar: number = 0;
  usuarios: Usuario[] = [];
  usuarioscopia: Usuario[] = [];
  carreras: string[] = []; 

  usuarioSeleccionado: Usuario = new Usuario();

  terminoBusqueda: string = '';

  constructor(private usuarioapi: UsuarioServiceAPI , private carrerasAPI : CarreraService) {
    super();
  }


  ngOnInit() {
    this.cargarUsuarios();
    this.cargarCarreras();
  }

  crearusuario() {
    this.botoncrear.set(true);
  }

  cargarCarreras() {
    this.carrerasAPI.obtenerCarreras().subscribe({
      next:(data: any[]) => {
        this.carreras = data.map(carrera => carrera.nombre);
      },
      error:(error) => {
        this.mensajeerror = 'Error al cargar las carreras, intente más tarde.';
        console.error('Error al cargar las carreras:', error);
        this.error.set(true);
      }
    });
  }



  cargarUsuarios() {
    this.usuarioapi.obtenerUsuarios().subscribe({
      next: (data: any[]) => {
        this.usuarios = data;
        this.usuarioscopia = [...this.usuarios]; // Guardar una copia para la búsqueda
      },
      error: (error) => {
        this.mensajeerror = 'Error al cargar los usuarios, intente más tarde.';
        console.error('Error al cargar los usuarios:', error);
        this.error.set(true);
      }
    });
  }

  actualizarTabla() {
    this.cargarUsuarios();
  }

  private normalizeText(text: string): string {
    if (typeof text !== 'string') {
      return String(text || '').toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
    }
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos
  }

  buscar() {
  if (this.terminoBusqueda.trim() === '') {
    this.limpiarBusqueda();
    return;
  }

  const busquedaNormalizada = this.normalizeText(this.terminoBusqueda);

  this.usuarios = this.usuarios.filter(usuario =>
    this.normalizeText(usuario.carnet || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.nombre || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.apellido_paterno || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.apellido_materno || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.correo || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.telefono || '').includes(busquedaNormalizada) ||
    this.normalizeText(usuario.carrera || '').includes(busquedaNormalizada)
  );
}

  limpiarBusqueda() {
    this.terminoBusqueda = '';
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
        this.usuarios.splice(this.valoreliminar, 1);
        this.usuarioscopia = [...this.usuarios];
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      },
      error: (error) => {
        this.mensajeerror='Error al eliminar el usuario, intente más tarde.';
        console.error('Error al eliminar usuario: ' + error);
        this.error.set(true);
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      }
    });
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.valoreliminar = 0;
  }



 
}
