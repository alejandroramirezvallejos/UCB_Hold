import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-usuarios-editar',
  imports: [FormsModule,MostrarerrorComponent , Aviso , AvisoExitoComponent],
  templateUrl: './usuarios-editar.component.html',
  styleUrl: './usuarios-editar.component.css'
})
export class UsuariosEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() usuario: Usuario = new Usuario();
  @Input() carreras: string[] = [];


  contrasena: string = '';

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
        this.mensajeerror="Error al editar el usuario";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
