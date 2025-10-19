import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { Usuario } from '../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../services/APIS/Usuario/usuario.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CarreraService } from '../../../../services/APIS/Carrera/carrera.service';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { MostrarerrorComponent } from '../../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-editar',
  imports: [CommonModule,FormsModule,AvisoExitoComponent , MostrarerrorComponent],
  templateUrl: './editar.component.html',
  styleUrl: './editar.component.css'
})
export class EditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Input() usuario: Usuario = new Usuario();

  exito : WritableSignal<boolean> = signal(false);
  mensajeexito : string = "";
  
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = "";

  carreras: string[] = [];


  contrasena: string = '';

  constructor(private usuarioApi: UsuarioServiceAPI , private carrerasAPI : CarreraService) {}

  ngOnInit() {
    this.cargarcarrera(); 
  }

  cargarcarrera() {
    this.carrerasAPI.obtenerCarreras().subscribe({
        next: (data) => {
          this.carreras = data.map(carrera => carrera.nombre);
        },
        error: (error) => {
          this.mensajeerror = "Error al obtener las carreras , intente mas tarde";
          console.error('Error al obtener las carreras:', error.error.mensaje);
          this.error.set(true);
        }
      });
  }



  confirmar() {
    this.usuarioApi.editarUsuario(this.usuario, this.contrasena).subscribe({
        next: (data) => {
          this.mensajeexito = "Usuario actualizado con exito";
          this.exito.set(true);
        },
        error: (error) => {
          this.mensajeerror = "Error al actualizar el usuario , intente mas tarde";
          console.error('Error al actualizar el usuario:', error.error.mensaje);
          this.error.set(true);
        }
    }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
