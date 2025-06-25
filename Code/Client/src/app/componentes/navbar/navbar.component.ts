// navbar.component.ts
import { Component, Output, EventEmitter, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
import { NotificacionService } from '../../services/APIS/Notificacion/notificacion.service';
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
  notificaciones: boolean = false;
  private intervalId: any;

  constructor(private carrito : CarritoService , private router : Router , private notificacionesAPI : NotificacionService , private usuario : UsuarioService) { }


  ngOnInit() {
    this.notificacionesAPI.enviarnotificaciones(); 
    this.verificarnotificaciones();
    this.intervalId = setInterval(() => {
      this.notificacionesAPI.enviarnotificaciones(); 
      this.verificarnotificaciones();
    }, 1000);
  }

  ngOnDestroy() { 
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }




  botonhome() {
   
    this.router.navigate(['/home']);
  }

  botonnotificaciones() {
    
    this.router.navigate(['/Notificaciones']);

  }

  toggleUserMenu() {
    this.showUserMenu.set(!this.showUserMenu());
  }

  totalproductos(): number {
    return this.carrito.obtenertotal();
  }

  mostrarcarrito() {
    this.router.navigate(['/Carrito']);
  }
  
  verificarnotificaciones() {
    if(this.usuario.vacio()){
      this.notificaciones = false;
      return;
    }
    
    this.notificacionesAPI.verificarnoleidas(this.usuario.obtenercarnet()).subscribe({
      next: (data) => {
        this.notificaciones = data;
      },
      error: (error) => {
        console.error('Error verificando notificaciones:', error);
        this.notificaciones = false;
      }
    });
  }

}
