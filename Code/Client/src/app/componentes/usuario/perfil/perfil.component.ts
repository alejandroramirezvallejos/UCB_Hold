import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { CarreraService } from '../../../services/APIS/Carrera/carrera.service';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { EditarComponent } from './editar/editar.component';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgOptimizedImage,NgOptimizedImage , EditarComponent],
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent  {

  editar :  WritableSignal<boolean> = signal(false);
  usuario: Usuario = new Usuario(); 
  constructor(
    private usuarioS: UsuarioService,
  ) 
  {
    this.usuario = this.usuarioS.obtenerDatosUsuario(); 
  }



 
  toggleEdit() {
    this.editar.set(!this.editar());
  }
  
}
