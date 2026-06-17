import { Component, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { EditarComponent } from './editar/editar.component';
@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditarComponent],
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent {
  editar: WritableSignal<boolean> = signal(false);
  usuario: Usuario = new Usuario();
  constructor(private readonly usuarioS: UsuarioService) {
    this.usuario = this.usuarioS.obtenerDatosUsuario();
  }
  toggleEdit() {
    this.editar.set(!this.editar());
  }
  onGuardado(actualizado: Usuario) {
    this.usuario = { ...actualizado };
  }
}
