import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../../services/APIS/Usuario/usuario.service';

@Component({
  selector: 'app-usuarios-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './usuarios-crear.component.html',
  styleUrl: './usuarios-crear.component.css'
})
export class UsuariosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  @Input() carreras: string[] = [];

  usuario: Usuario = new Usuario();

  contrasena: string = '';

  constructor(private usuarioApi: UsuarioServiceAPI) {}  // TODO : implementar
  registrar() {
    this.usuarioApi.registrarCuenta(this.usuario, this.contrasena, this.usuario.rol!).subscribe(
      response => {
        this.Actualizar.emit(); 
        this.cerrar();
      },
      error => {
        console.error('Error completo:', error);
        console.error('error.error:', error.error);
        console.error('error.error.error:', error.error?.error);
        console.error('error.error.mensaje:', error.error?.mensaje);
        
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
