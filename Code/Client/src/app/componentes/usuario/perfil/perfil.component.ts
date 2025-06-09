import { Component, OnInit } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgOptimizedImage,NgOptimizedImage],
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  profileForm!: FormGroup;
  editMode = false;

  constructor(
    private fb: FormBuilder,
    private usuarioS: UsuarioService
  ) {}

  ngOnInit() {
    this.profileForm = this.fb.group({
      nombre: [this.usuarioS.usuario.nombre || '', Validators.required],
      carnet: [this.usuarioS.usuario.carnet || ''],
      apellido_paterno: [this.usuarioS.usuario.apellido_paterno || ''],
      apellido_materno: [this.usuarioS.usuario.apellido_materno || ''],
      correo: [this.usuarioS.usuario.correo || '', [Validators.required, Validators.email]],
      telefono: [this.usuarioS.usuario.telefono || ''],
      nombre_referencia: [this.usuarioS.usuario.nombre_referencia || ''],
      telefono_referencia: [this.usuarioS.usuario.telefono_referencia || ''],
      email_referencia: [this.usuarioS.usuario.email_referencia || '']
    });
    this.profileForm.disable();
  }

  toggleEdit() {
    this.editMode = true;
    this.profileForm.enable();
  }

  saveProfile() {
    if (this.profileForm.valid) {
      const { nombre, carnet, apellido_paterno, apellido_materno, correo, telefono, nombre_referencia, telefono_referencia, email_referencia } = this.profileForm.value;
      // Asegurar que se guardan strings vacíos y no valores undefined o null
      const usuario : Usuario = {
        id : this.usuarioS.usuario.id,
        carnet: carnet || '',
        nombre: nombre || '',
        apellido_materno: apellido_materno || '',
        apellido_paterno: apellido_paterno || '',
        rol: this.usuarioS.usuario.rol || '',
        correo: correo || '',
        telefono: telefono || '',
        nombre_referencia: nombre_referencia || '',
        telefono_referencia: telefono_referencia || '',
        email_referencia: email_referencia || ''
      };
      this.usuarioS.usuario = usuario;
      this.profileForm.disable();
      this.editMode = false;
    } else {
      console.error('El formulario no es válido');
      // Marcar los campos con error para que el usuario pueda identificarlos
      Object.keys(this.profileForm.controls).forEach(key => {
        const control = this.profileForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }
}
