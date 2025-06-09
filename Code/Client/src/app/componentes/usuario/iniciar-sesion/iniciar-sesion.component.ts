import { Component } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-iniciar-sesion',
  standalone: true,
  imports: [FormsModule, CommonModule ],
  templateUrl: './iniciar-sesion.component.html',
  styleUrls: ['./iniciar-sesion.component.css']
})
export class IniciarSesionComponent {
  email : string = ""; 
  contrasena : string ="";
  loading: boolean = false;
  constructor(private usuario : UsuarioService , private router : Router){};
  
  login(){
    this.loading = true;
    this.usuario.iniciarsesion(this.email,this.contrasena, "administrador")
    this.router.navigate(["/home"])
  }

  registrarUsuario(){
    this.router.navigate(["/Registrar-Usuario"])
  }

  recuperarContrasena(){
    this.router.navigate(["/Recuperar-Contrasena"])
  }
  
}