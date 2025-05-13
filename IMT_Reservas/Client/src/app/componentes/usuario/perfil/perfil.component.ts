import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsuarioService } from '../../../services/usuario/usuario.service';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
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
      apellido_paterno: [this.usuario.apellido_paterno || ''],
      apellido_materno: [this.usuario.apellido_materno || ''],
      correo: [this.usuario.correo || '', [Validators.required, Validators.email]],
      telefono: [this.usuario.telefono || '']
    });
    this.profileForm.disable();
  }

  toggleEdit() {
    this.editMode = true;
    this.profileForm.enable();
  }

  saveProfile() {
    if (this.profileForm.valid) {
      const { nombre, apellido_paterno, apellido_materno, correo, telefono } = this.profileForm.value;
      this.usuario.nombre = nombre;
      this.usuario.apellido_paterno = apellido_paterno;
      this.usuario.apellido_materno = apellido_materno;
      this.usuario.correo = correo;
      this.usuario.telefono = telefono;
      this.profileForm.disable();
      this.editMode = false;
    }
  }
}
