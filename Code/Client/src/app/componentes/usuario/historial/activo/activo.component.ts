import { Component } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-activo',
  imports: [CommonModule, DatePipe],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css'
})
export class ActivoComponent {
 datos: Prestamos[] = [ ];

  constructor( private usuario : UsuarioService , private prestamoApi : PrestamosAPIService){}; 


  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , 'activo').subscribe({
      next: (data) => {
        this.datos = data;
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 

  }


}
