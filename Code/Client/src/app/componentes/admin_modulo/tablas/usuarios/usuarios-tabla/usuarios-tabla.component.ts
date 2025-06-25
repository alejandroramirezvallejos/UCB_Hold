import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuariosCrearComponent } from '../usuarios-crear/usuarios-crear.component';
import { UsuariosEditarComponent } from '../usuarios-editar/usuarios-editar.component';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';
import { CarreraService } from '../../../../../services/APIS/Carrera/carrera.service';

@Component({
  selector: 'app-usuarios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, UsuariosCrearComponent, UsuariosEditarComponent],
  templateUrl: './usuarios-tabla.component.html',
  styleUrls: ['./usuarios-tabla.component.css']
})
export class UsuariosTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  valoreliminar: number = 0;
  usuarios: Usuario[] = [];
  usuarioscopia: Usuario[] = [];
  carreras: string[] = []; 

  usuarioSeleccionado: Usuario = new Usuario();

  terminoBusqueda: string = '';

  constructor(private usuarioapi: UsuarioServiceAPI , private carrerasAPI : CarreraService) {}


  ngOnInit() {
    this.cargarUsuarios();
    this.cargarCarreras();
  }

  crearusuario() {
    this.botoncrear.set(true);
  }

  cargarCarreras() {
    this.carrerasAPI.obtenerCarreras().subscribe(
      (data: any[]) => {
        this.carreras = data.map(carrera => carrera.nombre);
      },
      (error) => {
        console.error('Error al cargar las carreras:', error);
      }
    );
  }



  cargarUsuarios() {
    this.usuarioapi.obtenerUsuarios().subscribe(
      (data: any[]) => {
        this.usuarios = data;
        this.usuarioscopia = [...this.usuarios]; // Guardar una copia para la búsqueda
      },
      (error) => {
        console.error('Error al cargar los usuarios:', error);
      }
    );
  }

  actualizarTabla() {
    this.cargarUsuarios();
  }
  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    this.usuarios = this.usuarios.filter(usuario =>
      (usuario.carnet || '') .toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      ((usuario.nombre || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.apellido_paterno || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.apellido_materno || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.correo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.telefono || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.rol || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.carrera || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.nombre_referencia || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())) ||
      ((usuario.telefono_referencia || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()))
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
    this.usuarioapi.eliminarUsuario(usuarioAEliminar.id || '').subscribe(
      response => {
        this.usuarios.splice(this.valoreliminar, 1);
        this.usuarioscopia = [...this.usuarios];
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      },
      error => {
        alert('Error al eliminar usuario: ' + error);
        this.alertaeliminar = false;
        this.valoreliminar = 0;
      }
    );
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.valoreliminar = 0;
  }



 
}
