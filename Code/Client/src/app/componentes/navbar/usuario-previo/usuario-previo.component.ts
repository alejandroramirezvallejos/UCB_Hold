import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';

@Component({
  selector: 'app-usuario-previo',
  imports: [],
  templateUrl: './usuario-previo.component.html',
  styleUrl: './usuario-previo.component.css'
})
export class UsuarioPrevioComponent {

  sesion :boolean;
  rol : string;
  @Input() showUserMenu : WritableSignal<Boolean> = signal(true);


  constructor(private router : Router , private usuario : UsuarioService){
    this.sesion=!usuario.vacio() ;
    this.rol=usuario.obtenerrol();
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
        this.usuario.vaciar();
      }
      else if(item=='admin'){
        this.router.navigate(["/admin"])
      }

      this.showUserMenu.set(false);
  }
}
