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
  @Input() usuario: Usuario = new Usuario();
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
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
