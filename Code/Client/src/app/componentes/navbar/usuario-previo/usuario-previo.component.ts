import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { AuthService } from '../../../services/auth/auth.service';
@Component({
  selector: 'app-usuario-previo',
  imports: [],
  templateUrl: './usuario-previo.component.html',
  styleUrl: './usuario-previo.component.css'
})
export class UsuarioPrevioComponent {
  sesion :boolean;
  rol : string;
  isInAdminMode: boolean = false;
  @Input() showUserMenu : WritableSignal<Boolean> = signal(true);
  constructor(private readonly router: Router, private usuario: UsuarioService, private authService: AuthService){
    this.sesion=!usuario.vacio();
    this.rol=usuario.obtenerrol();
    this.isInAdminMode = this.router.url.includes('/admin');
  }
    seleccionar(item: string) {
      if(this.usuario.vacio()==true){
         this.router.navigate(["/Iniciar-Sesion"])
      }
      else if (item=="iniciarsesion"){
         this.router.navigate(["/Iniciar-Sesion"])
      }
      else if(item=='perfil'){
       this.router.navigate(["/Perfil"])
      }
      else if(item=='historial'){
        this.router.navigate(["/Historial"])
      }
      else if (item=='cerrar-sesion'){
        this.authService.clear();
        this.usuario.vaciar();
        this.router.navigate(["/Iniciar-Sesion"]);
      }
      else if(item=='admin'){
        this.router.navigate(["/admin"])
      }
      else if(item=='modousuario'){
        this.router.navigate(["/home"])
      }
      this.showUserMenu.set(false);
  }
}
