import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cambiar-contrasena',
  imports: [FormsModule],
  templateUrl: './cambiar-contrasena.component.html',
  styleUrl: './cambiar-contrasena.component.css'
})
export class CambiarContrasenaComponent {
  nuevaContrasena: string = '';
  confirmacionContrasena: string = '';

  constructor(private router: Router) {}

  cambiarContrasena() {
    if (this.nuevaContrasena === this.confirmacionContrasena) {
      // Aquí se llamaría al servicio para cambiar la contraseña

      alert('Contraseña cambiada exitosamente');
      this.router.navigate(['/Iniciar-Sesion']);
    } else {
      alert('Las contraseñas no coinciden');
    }
  }
}
