import { Component, signal, WritableSignal } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { MostrarerrorComponent } from '../../mostrarerror/mostrarerror.component';
import { NotificacionService } from '../../../services/APIS/Notificacion/notificacion.service';


@Component({
  selector: 'app-iniciar-sesion',
  standalone: true,
  imports: [FormsModule, CommonModule ,MostrarerrorComponent],
  templateUrl: './iniciar-sesion.component.html',
  styleUrls: ['./iniciar-sesion.component.css']
})
export class IniciarSesionComponent {
  email : string = ""; 
  contrasena : string ="";
  loading: boolean = false;
  incorrecto : boolean = false;
  errorraro : WritableSignal<number> = signal(0);
  constructor(private usuario : UsuarioService , private router : Router , private usuarioapi : UsuarioServiceAPI ){};

  login(){
    this.loading = true;
    this.usuarioapi.iniciarsesion(this.email, this.contrasena).subscribe(
      (data) => {
        this.usuario.iniciarsesion(data);
        this.loading = false;
        this.incorrecto = false;

        this.router.navigate(["/home"]);

      },
      (error) => {
        if(error.status === 400 || error.status === 401){
          this.incorrecto = true;
         
        }
        else{
          this.errorraro.set(1); 
          
        }
        this.loading = false;
      }
    );
  }

  registrarUsuario(){
    this.router.navigate(["/Registrar-Usuario"])
  }


}