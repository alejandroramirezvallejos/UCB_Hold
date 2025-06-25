import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { Usuario } from '../../../../models/usuario';
import { UsuarioServiceAPI } from '../../../../services/APIS/Usuario/usuario.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CarreraService } from '../../../../services/APIS/Carrera/carrera.service';

@Component({
  selector: 'app-editar',
  imports: [CommonModule,FormsModule],
  templateUrl: './editar.component.html',
  styleUrl: './editar.component.css'
})
export class EditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Input() usuario: Usuario = new Usuario();
  
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
          console.error('Error al obtener las carreras:', error.error.mensaje);

        }
      });
  }



  confirmar() {
    this.usuarioApi.editarUsuario(this.usuario, this.contrasena).subscribe({
        next: (data) => {
          this.botoneditar.set(false);
        },
        error: (error) => {
          console.error('Error al actualizar el usuario:', error.error.mensaje);
        }
    }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
