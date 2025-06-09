import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registrar-usuario',
  imports: [FormsModule, CommonModule],
  templateUrl: './registrar-usuario.component.html',
  styleUrl: './registrar-usuario.component.css'
})
export class RegistrarUsuarioComponent {
  nuevoUsuario: Usuario = new Usuario();
  password: string = '';
  confirmPassword: string = '';

  constructor(private usuarioS: UsuarioService, private router: Router) {}

  // TODO : mandar a la base de datos
  registrar() {
    this.nuevoUsuario.rol = 'usuario';

    this.usuarioS.usuario = this.nuevoUsuario;

    
    this.router.navigate(['/home']);
  }

  irALogin() {
    this.router.navigate(['/Iniciar-Sesion']);
  }
  validartelefono(telefono : string ) : boolean{
    const regex = /^[-+0-9]+$/;
   return !regex.test(telefono);
  }
}
