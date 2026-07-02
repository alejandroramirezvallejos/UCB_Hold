import { Component, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { UsuarioService } from '@entities/user';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '@entities/user';
import { AuthService } from '@features/auth-session';
import { MostrarerrorComponent } from '@shared/ui';

const AUTH_REDIRECT_CHECK_INTERVAL_MS = 50;
const AUTH_REDIRECT_TIMEOUT_MS = 2000;
const BAD_REQUEST_STATUS = 400;
const UNAUTHORIZED_STATUS = 401;

@Component({
  selector: 'app-iniciar-sesion',
  standalone: true,
  imports: [FormsModule, CommonModule, MostrarerrorComponent],
  templateUrl: './iniciar-sesion.component.html',
  styleUrls: ['./iniciar-sesion.component.css'],
})
export class IniciarSesionComponent {
  email: string = '';
  contrasena: string = '';
  loading: boolean = false;
  incorrecto: boolean = false;
  mostrarContrasena = false;
  errorraro: WritableSignal<boolean> = signal(false);

  constructor(
    private readonly usuario: UsuarioService,
    private readonly router: Router,
    private readonly usuarioapi: UsuarioServiceAPI,
    private readonly authService: AuthService,
  ) {}

  login() {
    this.loading = true;
    this.usuarioapi.iniciarSesion(this.email, this.contrasena).subscribe({
      next: (data) => {
        this.authService.setSession(
          data.accessToken,
          data.refreshToken,
          data.usuario,
        );
        this.usuario.guardarSesion(data.usuario);

        const checkInterval = setInterval(() => {
          if (this.authService.getAccessToken()) {
            clearInterval(checkInterval);
            this.loading = false;
            this.incorrecto = false;
            this.router.navigate(['/home']);
          }
        }, AUTH_REDIRECT_CHECK_INTERVAL_MS);

        setTimeout(() => {
          clearInterval(checkInterval);
          if (this.loading) {
            this.loading = false;
            this.incorrecto = false;
            this.router.navigate(['/home']);
          }
        }, AUTH_REDIRECT_TIMEOUT_MS);
      },
      error: (error) => {
        if (
          error.status === BAD_REQUEST_STATUS ||
          error.status === UNAUTHORIZED_STATUS
        ) {
          this.incorrecto = true;
        } else {
          this.errorraro.set(true);
        }
        this.loading = false;
      },
    });
  }
  registrarUsuario() {
    this.router.navigate(['/Registrar-Usuario']);
  }

  alternarVisibilidadContrasena(): void {
    this.mostrarContrasena = !this.mostrarContrasena;
  }
}
