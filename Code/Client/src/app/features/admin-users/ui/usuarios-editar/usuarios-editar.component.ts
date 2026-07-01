import {
  Component,
  HostListener,
  EventEmitter,
  Input,
  Output,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '@entities/user';
import { UsuarioServiceAPI } from '@entities/user';
import { BaseTablaComponent } from '@shared/lib/admin-table';
import { MostrarerrorComponent } from '@shared/ui';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
import { CustomSelectComponent, OpcionSelect } from '@shared/ui';
@Component({
  selector: 'app-usuarios-editar',
  imports: [
    FormsModule,
    MostrarerrorComponent,
    Aviso,
    AvisoExitoComponent,
    CustomSelectComponent,
  ],
  templateUrl: './usuarios-editar.component.html',
  styleUrl: './usuarios-editar.component.css',
})
export class UsuariosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() usuario: Usuario = new Usuario();
  @Input() carreras: string[] = [];
  contrasena: string = '';
  rolesOpciones: OpcionSelect[] = [
    { value: 'administrador', label: 'Administrador' },
    { value: 'docente', label: 'Docente' },
    { value: 'estudiante', label: 'Estudiante' },
  ];
  constructor(private readonly usuarioApi: UsuarioServiceAPI) {
    super();
  }
  validareditar() {
    this.mensajeaviso = '¿Desea guardar los cambios realizados al usuario ?';
    this.aviso.set(true);
  }
  confirmar() {
    this.usuarioApi.editarUsuario(this.usuario, this.contrasena).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.mensajeexito = 'Usuario editado con exito';
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al editar el usuario',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }
  cerrar() {
    this.botoneditar.set(false);
  }
  @HostListener('click', ['$event'])
  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) this.cerrar();
  }
}
