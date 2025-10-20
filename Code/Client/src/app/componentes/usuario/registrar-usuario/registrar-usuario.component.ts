import { Component, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { Usuario } from '../../../models/usuario';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UsuarioServiceAPI } from '../../../services/APIS/Usuario/usuario.service';
import { CarreraService } from '../../../services/APIS/Carrera/carrera.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../pantallas_avisos/aviso-exito/aviso-exito.component';


@Component({
  selector: 'app-registrar-usuario',
  imports: [FormsModule, CommonModule , MostrarerrorComponent,AvisoExitoComponent],
  templateUrl: './registrar-usuario.component.html',
  styleUrl: './registrar-usuario.component.css'
})
export class RegistrarUsuarioComponent {
  nuevoUsuario: Usuario = new Usuario();
  password: string = '';
  confirmPassword: string = '';
  carreras: string[] = []; 

  error : WritableSignal<boolean> = signal(false);
  mensajeerror : string = "";

  aviso : WritableSignal<boolean> = signal(false);
  mensajeaviso : string = "Aviso desconocido , si ve esto es un error , avise al soporte si puede o intente mas tarde";


  constructor(private usuarioS: UsuarioService, private router: Router , private registrarcuenta : UsuarioServiceAPI , private carrerasS : CarreraService) {}


  ngOnInit() {
    this.carrerasS.obtenerCarreras().subscribe({
      next: (response) => {
         this.carreras = response.map(carrera => carrera.nombre); 
         
      },
      error: (error) => {
       
        this.mensajeerror = "Error al obtener las carreras intente mas tarde";
        console.error('Error al obtener las carreras:', error.error.mensaje);
        this.error.set(true);
      
      }

    });

  }





  registrar() {
    this.nuevoUsuario.rol = 'usuario';
    
    this.registrarcuenta.registrarCuenta(this.nuevoUsuario,this.password, "estudiante").subscribe({
      next: (response) => {
        this.mensajeaviso = "Usuario registrado exitosamente";
        this.usuarioS.usuario = this.nuevoUsuario;
        this.aviso.set(true);
      },
      error: (error) => {
        this.mensajeerror = "Error al registrar el usuario intente mas tarde";
        alert('Error al registrar el usuario:' + error.error.mensaje);
        this.error.set(true);
      }
    });

    
  }

  


  irALogin() {
    this.router.navigate(['/home']);
  }
  validartelefono(telefono : string ) : boolean{
    const regex = /^[-+0-9]+$/;
   return !regex.test(telefono);
  }
}
