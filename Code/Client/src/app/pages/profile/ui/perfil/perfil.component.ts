import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Usuario, UsuarioService } from '@entities/user';
import { EditarComponent } from '@features/profile-edit';
@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditarComponent],
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css'],
})
export class PerfilComponent {
  editar: WritableSignal<boolean> = signal(false);
  usuario: Usuario = new Usuario();
  constructor(private readonly usuarioS: UsuarioService) {
    this.usuario = this.usuarioS.obtenerUsuario();
  }
  toggleEdit() {
    this.editar.set(!this.editar());
  }
  onGuardado(actualizado: Usuario) {
    this.usuario = { ...actualizado };
  }
}
