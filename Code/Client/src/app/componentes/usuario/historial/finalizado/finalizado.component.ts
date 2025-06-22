import { Component } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-finalizado',
  imports: [CommonModule],
  templateUrl: './finalizado.component.html',
  styleUrl: './finalizado.component.css'
})
export class FinalizadoComponent {
  datos  = new Map<number, PrestamoAgrupados>;

  constructor( private usuario : UsuarioService , private prestamoApi : PrestamosAPIService){}; 


  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.usuario.id! , 'finalizado').subscribe({
      next: (data) => {
        this.agruparPrestamos(data);
      },
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }

    }); 
}

  agruparPrestamos(datos: Prestamos[]) {
    this.datos.clear();
    for (let prestamo of datos) {
      if( this.datos.has(prestamo.Id!)) {
        this.datos.get(prestamo.Id)!.insertarEquipo(prestamo);
      }
      else{
        this.datos.set(prestamo.Id! , new PrestamoAgrupados([prestamo]));
      }
    }

  }

}
