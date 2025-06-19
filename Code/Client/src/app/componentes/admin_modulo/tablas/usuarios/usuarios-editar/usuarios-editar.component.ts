import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';

@Component({
  selector: 'app-usuarios-editar',
  imports: [FormsModule],
  templateUrl: './usuarios-editar.component.html',
  styleUrl: './usuarios-editar.component.css'
})
export class UsuariosEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() usuario: Usuario = {
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
  @Input() carreras: string[] = [];


  contrasena: string = '';

  constructor(private usuarioApi: UsuarioServiceAPI) {}

  confirmar() {
    this.usuarioApi.editarUsuario(this.usuario, this.contrasena).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
