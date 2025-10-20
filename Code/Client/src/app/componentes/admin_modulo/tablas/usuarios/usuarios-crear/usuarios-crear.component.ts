import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-usuarios-crear',
  standalone: true,
  imports: [FormsModule, MostrarerrorComponent,Aviso , AvisoExitoComponent],
  templateUrl: './usuarios-crear.component.html',
  styleUrl: './usuarios-crear.component.css'
})
export class UsuariosCrearComponent extends BaseTablaComponent{

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  @Input() carreras: string[] = [];

  usuario: Usuario = new Usuario();

  contrasena: string = '';

  constructor(private usuarioApi: UsuarioServiceAPI) {
    super();
  } 
  
  validarcrear(){
    this.mensajeaviso="¿Desea crear el usuario ";
    this.aviso.set(true);
  }
  

  registrar() {
    this.usuarioApi.registrarCuenta(this.usuario, this.contrasena, this.usuario.rol!).subscribe({
      next : (response) => {
        this.Actualizar.emit(); 
        this.mensajeexito="Usuario creado exitosamente";
        this.exito.set(true);
      },
      error: (error) => {
        console.error('Error completo:', error);
        console.error('error.error:', error.error);
        console.error('error.error.error:', error.error?.error);
        console.error('error.error.mensaje:', error.error?.mensaje);
        
        this.mensajeerror="No se pudo crear el usuario. ";
        this.error.set(true);

        console.error(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
