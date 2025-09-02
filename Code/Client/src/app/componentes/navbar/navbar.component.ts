// navbar.component.ts
import { Component, Output, EventEmitter, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
import { UsuarioService } from '../../services/usuario/usuario.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule  , CommonModule, UsuarioPrevioComponent],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})


export class NavbarComponent {
 
  showUserMenu : WritableSignal<boolean> = signal(false);


  constructor(private carrito : CarritoService , private router : Router , private usuario : UsuarioService) { }

  botonhome() {
    if(this.usuario.vacio()==true){
      this.router.navigate(['/Iniciar-Sesion']);
    }
    else{
       this.router.navigate(['/home']);
    }


   
  }



  toggleUserMenu() {
      this.showUserMenu.set(!this.showUserMenu());
  }

  totalproductos(): number {
    return this.carrito.obtenertotal();
  }

  mostrarcarrito() {
    if(this.usuario.vacio()==true){
      this.router.navigate(['/Iniciar-Sesion']);
    }
    else{
      this.router.navigate(['/Carrito']);
    }

  }
  


}
