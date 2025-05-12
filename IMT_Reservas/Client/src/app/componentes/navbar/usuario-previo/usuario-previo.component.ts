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

  @Input() showUserMenu : WritableSignal<Boolean> = signal(true); 

  
  constructor(private router : Router , private usuario : UsuarioService){}

    seleccionar(item: string) {
      if(this.usuario.vacio()==true){
         this.router.navigate(["/Iniciar-Sesion"])
      }
      else if(item=='perfil'){
        //routa del perfil 
      }
      else if(item=='admin'){
        this.router.navigate(["/admin"])
      }
    
    
    
      this.showUserMenu.set(false); 
  }
}
