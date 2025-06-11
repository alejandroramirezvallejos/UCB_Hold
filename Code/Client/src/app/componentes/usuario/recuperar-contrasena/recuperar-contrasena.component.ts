import { Component, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CodigoUnicoComponent } from './codigo-unico/codigo-unico.component';

@Component({
  selector: 'app-recuperar-contrasena',
  standalone: true,
  imports: [FormsModule, CommonModule , CodigoUnicoComponent],
  templateUrl: './recuperar-contrasena.component.html',
  styleUrl: './recuperar-contrasena.component.css'
})
export class RecuperarContrasenaComponent {
  correo: string = '';
  boton: WritableSignal<boolean> =  signal(false);

  constructor(private router: Router) {}

 
  enviarCorreo() {
    if (this.correo) {
        this.boton.set(true);
    }
  }

  volverLogin() {
    this.router.navigate(['/Iniciar-Sesion']);
  }
}
