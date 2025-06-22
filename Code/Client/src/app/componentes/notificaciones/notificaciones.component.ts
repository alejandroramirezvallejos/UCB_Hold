import { Component } from '@angular/core';
import { Notificacion } from '../../models/Notificacion';
import { NotificacionService } from '../../services/APIS/Notificacion/notificacion.service';
import { UsuarioService } from '../../services/usuario/usuario.service';

@Component({
  selector: 'app-notificaciones',
  imports: [],
  templateUrl: './notificaciones.component.html',
  styleUrl: './notificaciones.component.css'
})
export class NotificacionesComponent {

  notificaciones: Notificacion[] = [];

  constructor(private notificacionAPI :  NotificacionService , private usuario : UsuarioService) {}; 

  ngOnInit() {

    this.cargarNotificaciones();

  }


  cargarNotificaciones() {
    this.notificacionAPI.obtenerNotificaciones(this.usuario.obtenercarnet()).subscribe({
      next: (data) => {
        this.notificaciones = data;
      },
      error: (error) => {
        alert( error.error.error +': ' + error.error.mensaje);
      }
    });
  }

  leerNotificacion(id : string ) {
    this.notificacionAPI.leerNotificacion(id).subscribe({
      next: (data) => {
        this.cargarNotificaciones();
      },
      error: (error) => {
        alert( error.error.error +': '+ error.error.mensaje);
      }
    });
  }


}
