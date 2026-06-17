import { Component, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { AuthService } from '../../../services/auth/auth.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
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
  errorraro : WritableSignal<boolean> = signal(false);

  constructor(
    private readonly usuario    : UsuarioService,
    private readonly router     : Router,
    private readonly usuarioapi : UsuarioServiceAPI,
    private readonly authService: AuthService
  ) {}

  
  login(){
    this.loading = true;
    this.usuarioapi.iniciarsesion(this.email, this.contrasena).subscribe({
      next: (data) => {
        this.authService.setSession(data.accessToken, data.refreshToken, data.usuario);
        this.usuario.iniciarsesion(data.usuario);
        
        const checkInterval = setInterval(() => {
          if (this.authService.getAccessToken()) {
            clearInterval(checkInterval);
            this.loading = false;
            this.incorrecto = false;
            this.router.navigate(["/home"]);
          }
        }, 50);

        setTimeout(() => {
          clearInterval(checkInterval);
          if (this.loading) {
            this.loading = false;
            this.incorrecto = false;
            this.router.navigate(["/home"]);
          }
        }, 2000);
      },
      error: (error) => {
        if(error.status === 400 || error.status === 401){
          this.incorrecto = true;
        }
        else{
          this.errorraro.set(true);
        }
        this.loading = false;
      }
    });
  }
  registrarUsuario(){
    this.router.navigate(["/Registrar-Usuario"])
  }
}
