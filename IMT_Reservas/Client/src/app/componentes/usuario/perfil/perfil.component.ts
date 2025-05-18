import { Component, OnInit } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsuarioService } from '../../../services/usuario/usuario.service';


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
    private usuario: UsuarioService
  ) {}

  ngOnInit() {
    this.profileForm = this.fb.group({
      nombre: [this.usuario.nombre || '', Validators.required],
      carnet: [this.usuario.carnet || ''],
      apellido_paterno: [this.usuario.apellido_paterno || ''],
      apellido_materno: [this.usuario.apellido_materno || ''],
      correo: [this.usuario.correo || '', [Validators.required, Validators.email]],
      telefono: [this.usuario.telefono || ''],
      nombre_referencia: [this.usuario.nombre_referencia || ''],
      telefono_referencia: [this.usuario.telefono_referencia || ''],
      email_referencia: [this.usuario.email_referencia || '']
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
      this.usuario.nombre = nombre || '';
      this.usuario.carnet = carnet || '';
      this.usuario.apellido_paterno = apellido_paterno || '';
      this.usuario.apellido_materno = apellido_materno || '';
      this.usuario.correo = correo || '';
      this.usuario.telefono = telefono || '';
      this.usuario.nombre_referencia = nombre_referencia || '';
      this.usuario.telefono_referencia = telefono_referencia || '';
      this.usuario.email_referencia = email_referencia || '';

      console.log('Perfil guardado:', this.usuario);
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
