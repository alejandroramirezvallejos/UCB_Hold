import { Component, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { CarreraService } from '../../../services/APIS/Carrera/carrera.service';
import { AvisoExitoComponent } from '../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { extractErrorMessage } from '../../../utils/error-handler';
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
        const errorMsg = extractErrorMessage(error, 'Error al obtener las carreras intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error('Error al obtener las carreras:', errorMsg);
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
        const errorMsg = extractErrorMessage(err, 'Error al registrar el usuario intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error('Error al registrar el usuario:', errorMsg);
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
