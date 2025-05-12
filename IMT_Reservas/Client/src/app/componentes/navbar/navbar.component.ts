// navbar.component.ts
import { Component, Output, EventEmitter, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CarritoPrevioComponent } from './carrito-previo/carrito-previo.component'
import { CommonModule } from '@angular/common';
import { BuscadorService } from '../../services/buscador/buscador.service';
import { Router } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule , CarritoPrevioComponent , CommonModule, UsuarioPrevioComponent],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})


export class NavbarComponent {
 
  showUserMenu : WritableSignal<boolean> = signal(false);
  showCarrito: WritableSignal<boolean> = signal(false);
  constructor(private carrito : CarritoService, private buscador : BuscadorService , private router : Router) { }


  botonhome() {
    this.buscador.reiniciar();
    this.router.navigate(['/home']);
  }



  toggleUserMenu() {
    this.showUserMenu.set(!this.showUserMenu());
  }



  totalproductos() : number   {
    return this.carrito.obtenertotal();
  }

  mostrarcarrito() {
    this.showCarrito.set(!this.showCarrito());
  }
}
