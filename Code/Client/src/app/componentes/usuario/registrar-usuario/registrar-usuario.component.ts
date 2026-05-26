import { Component, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { CarreraService } from '../../../services/APIS/Carrera/carrera.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../pantallas_avisos/aviso-exito/aviso-exito.component';
@Component({
  selector: 'app-registrar-usuario',
  imports: [FormsModule, CommonModule, MostrarerrorComponent, AvisoExitoComponent],
  templateUrl: './registrar-usuario.component.html',
  styleUrl: './registrar-usuario.component.css'
})
export class RegistrarUsuarioComponent {
  nuevoUsuario: Usuario = new Usuario();
  password: string = '';
  confirmPassword: string = '';
  carreras: string[] = [];
  isOpen: boolean = false;
  isHovered: boolean = false;
  submitted: boolean = false;
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = "";
  aviso: WritableSignal<boolean> = signal(false);
  mensajeaviso: string = "Aviso desconocido , si ve esto es un error , avise al soporte si puede o intente mas tarde";
  constructor(private usuarioS: UsuarioService, private router: Router, private registrarcuenta: UsuarioServiceAPI, private carrerasS: CarreraService) { }
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
      next: (response) => {
        this.carreras = response.map((carrera: any) => carrera.nombre);
      },
      error: (error) => {
        this.mensajeerror = "Error al obtener las carreras intente mas tarde";
        console.error('Error al obtener las carreras:', error?.error?.message);
        this.error.set(true);
      }
    });
  }
  registrar(form: any) { // Change signature to accept form
    this.submitted = true;
    if (form.invalid || this.password !== this.confirmPassword || this.validartelefono(this.nuevoUsuario.telefono) || !this.nuevoUsuario.carrera) {
      return;
    }
    this.nuevoUsuario.rol = 'usuario';
    this.registrarcuenta.registrarCuenta(this.nuevoUsuario, this.password, "estudiante").subscribe({
      next: (response) => {
        this.mensajeaviso = "Usuario registrado exitosamente";
        this.usuarioS.iniciarsesion(this.nuevoUsuario);
        this.aviso.set(true);
      },
      error: (err) => {
        if (err?.error?.Errors && err.error.Errors.length > 0) {
          this.mensajeerror = err.error.Errors[0];
        } else if (err?.error?.errors && err.error.errors.length > 0) {
          this.mensajeerror = err.error.errors[0];
        } else if (err?.error?.message) {
          this.mensajeerror = err.error.message;
        } else if (typeof err?.error === 'string') {
          this.mensajeerror = err.error;
        } else {
          this.mensajeerror = "Error al registrar el usuario intente mas tarde";
        }
        console.error('Error al registrar el usuario:', err);
        this.error.set(true);
      }
    });
  }
  irALogin() {
    this.router.navigate(['/Iniciar-Sesion']);
  }
  validartelefono(telefono: string | null | undefined): boolean {
    const regex = /^[-+0-9]+$/;
    return !regex.test(<string>telefono);
  }
}
