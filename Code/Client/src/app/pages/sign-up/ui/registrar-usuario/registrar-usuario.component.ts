import { Component, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '@entities/user';
import { Usuario } from '@entities/user';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '@entities/user';
import { CarreraService } from '@entities/career';
import { Carrera } from '@entities/admin';
import { AvisoExitoComponent } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
@Component({
  selector: 'app-registrar-usuario',
  imports: [
    FormsModule,
    CommonModule,
    MostrarerrorComponent,
    AvisoExitoComponent,
  ],
  templateUrl: './registrar-usuario.component.html',
  styleUrl: './registrar-usuario.component.css',
})
export class RegistrarUsuarioComponent {
  nuevoUsuario: Usuario = new Usuario();
  password: string = '';
  confirmPassword: string = '';
  mostrarPassword = false;
  mostrarConfirmPassword = false;
  carreras: string[] = [];
  isOpen: boolean = false;
  isHovered: boolean = false;
  submitted: boolean = false;
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = '';
  aviso: WritableSignal<boolean> = signal(false);
  mensajeaviso: string =
    'Aviso desconocido , si ve esto es un error , avise al soporte si puede o intente mas tarde';
  constructor(
    private readonly usuarioS: UsuarioService,
    private router: Router,
    private registrarcuenta: UsuarioServiceAPI,
    private carrerasS: CarreraService,
  ) {}
  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }
  selectCarrera(carrera: string) {
    this.nuevoUsuario.carrera = carrera;
    this.isOpen = false;
  }
  onMouseEnter() {
    this.isHovered = true;
  }
  onMouseLeave() {
    this.isHovered = false;
  }
  ngOnInit() {
    this.carrerasS.obtenerCarreras().subscribe({
      next: (response: Carrera[]) => {
        this.carreras = response.map((carrera) => carrera.Nombre ?? '');
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al obtener las carreras intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  registrar(form: NgForm) {
    this.submitted = true;
    if (
      form.invalid ||
      this.password !== this.confirmPassword ||
      this.validartelefono(this.nuevoUsuario.telefono) ||
      !this.nuevoUsuario.carrera
    ) {
      return;
    }
    this.nuevoUsuario.rol = 'usuario';
    this.registrarcuenta
      .registrarCuenta(this.nuevoUsuario, this.password, 'estudiante')
      .subscribe({
        next: (response) => {
          this.mensajeaviso = 'Usuario registrado exitosamente';
          this.usuarioS.guardarSesion(this.nuevoUsuario);
          this.aviso.set(true);
        },
        error: (err) => {
          const errorMsg = extractErrorMessage(
            err,
            'Error al registrar el usuario intente mas tarde',
          );
          this.mensajeerror = errorMsg;
          this.error.set(true);
        },
      });
  }
  irALogin() {
    this.router.navigate(['/Iniciar-Sesion']);
  }

  alternarVisibilidadPassword(): void {
    this.mostrarPassword = !this.mostrarPassword;
  }

  alternarVisibilidadConfirmPassword(): void {
    this.mostrarConfirmPassword = !this.mostrarConfirmPassword;
  }

  validartelefono(telefono: string | null | undefined): boolean {
    const regex = /^[-+0-9]+$/;
    return !regex.test(<string>telefono);
  }
}
