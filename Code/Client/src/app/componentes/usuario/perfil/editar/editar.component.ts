import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { Usuario } from '../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../services/APIS/Usuario/usuario.service';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CarreraService } from '../../../../services/APIS/Carrera/carrera.service';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { MostrarerrorComponent } from '../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { extractErrorMessage } from '../../../../utils/error-handler';
import { PantallaCargaComponent } from '../../../pantallas_avisos/pantalla-carga/pantalla-carga.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-editar',
  imports: [CommonModule, FormsModule, AvisoExitoComponent, MostrarerrorComponent, PantallaCargaComponent],
  templateUrl: './editar.component.html',
  styleUrl: './editar.component.css'
})
export class EditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Input() usuario: Usuario = new Usuario();
  @Output() guardado = new EventEmitter<Usuario>();
  localUsuario: Usuario = new Usuario();
  exito: WritableSignal<boolean> = signal(false);
  mensajeexito: string = '';
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = '';
  carreras: string[] = [];
  isOpen: boolean = false;
  isHovered: boolean = false;
  contrasena: string = '';
  cargando: boolean = false;

  constructor(
    private usuarioApi: UsuarioServiceAPI,
    private carrerasAPI: CarreraService,
    private usuarioStore: UsuarioService
  ) {}
  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }
  selectCarrera(carrera: string) {
    this.localUsuario.carrera = carrera;
    this.isOpen = false;
  }
  onMouseEnter() {
    this.isHovered = true;
  }
  onMouseLeave() {
    this.isHovered = false;
  }
  ngOnInit() {
    this.localUsuario = { ...this.usuario };
    this.cargarcarrera();
  }
  cargarcarrera() {
    this.carrerasAPI.obtenerCarreras().subscribe({
      next: (data) => {
        this.carreras = data.map((carrera: any) => carrera.nombre);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al obtener las carreras, intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error('Error al obtener las carreras:', errorMsg);
        this.error.set(true);
      }
    });
  }
  confirmar() {
    this.cargando = true;
    this.usuarioApi.editarUsuario(this.localUsuario, this.contrasena)
    .pipe(finalize(() => this.cargando = false))
    .subscribe({
      next: () => {
        this.usuarioStore.actualizarDatos({ ...this.localUsuario });
        this.guardado.emit({ ...this.localUsuario });
        this.mensajeexito = 'Perfil actualizado correctamente';
        this.exito.set(true);
        setTimeout(() => this.cerrar(), 2000);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al actualizar el usuario, intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error('Error al actualizar el usuario:', errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar() {
    this.botoneditar.set(false);
  }
}
