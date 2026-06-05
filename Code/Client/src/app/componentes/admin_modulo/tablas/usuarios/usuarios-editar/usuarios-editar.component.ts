import { Component, HostListener, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { CustomSelectComponent, OpcionSelect } from '../../../../compartidos/custom-select/custom-select.component';
@Component({
  selector: 'app-usuarios-editar',
  imports: [FormsModule,MostrarerrorComponent , Aviso , AvisoExitoComponent, CustomSelectComponent],
  templateUrl: './usuarios-editar.component.html',
  styleUrl: './usuarios-editar.component.css'
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
  constructor(private usuarioApi: UsuarioServiceAPI) {
    super();
  }
  validareditar(){
    this.mensajeaviso="¿Desea guardar los cambios realizados al usuario ?";
    this.aviso.set(true);
  }
  confirmar() {
    this.usuarioApi.editarUsuario(this.usuario, this.contrasena).subscribe({
      next: (response )=> {
        this.actualizar.emit();
        this.mensajeexito="Usuario editado con exito";
        this.exito.set(true);
      },
       error : (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al editar el usuario');
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
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