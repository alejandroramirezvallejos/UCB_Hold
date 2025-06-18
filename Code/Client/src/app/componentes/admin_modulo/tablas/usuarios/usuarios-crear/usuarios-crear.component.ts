import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';

@Component({
  selector: 'app-usuarios-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './usuarios-crear.component.html',
  styleUrl: './usuarios-crear.component.css'
})
export class UsuariosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  @Input() carreras: string[] = [];

  usuario: Usuario = {
    id: '',
    carnet: '',
    nombre: '',
    apellido_materno: '',
    apellido_paterno: '',
    rol: '',
    carrera_Id: 0,
    carrera: '',
    correo: '',
    telefono: '',
    nombre_referencia: '',
    telefono_referencia: '',
    email_referencia: ''
  };

  contrasena: string = '';

  constructor(private usuarioApi: UsuarioServiceAPI) {}

  // TODO : implementar
  registrar() {
    this.usuarioApi.registrarCuenta(this.usuario, this.contrasena).subscribe(
      response => {
        this.Actualizar.emit(); 
        this.cerrar();
      },
      error => {
        alert('Error al crear usuario: ' + error);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
