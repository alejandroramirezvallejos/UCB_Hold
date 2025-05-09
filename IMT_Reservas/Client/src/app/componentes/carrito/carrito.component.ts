import { Component } from '@angular/core';
import { CarritoService } from '../../services/carrito/carrito.service';
import {Carrito } from '../../models/carrito'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MandarcarritoService } from '../../services/enviar/carrito/mandarcarrito.service';

@Component({
  selector: 'app-carrito',
  standalone: true ,
  imports: [CommonModule,FormsModule],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css'
})
export class CarritoComponent {
  private error : boolean = false;
  hoy : Date= new Date();
 

  carrito: Carrito = {};
  constructor(private carritoS: CarritoService , private MandarCarrito : MandarcarritoService ) {
    this.carrito  = this.carritoS.obtenercarrito();
    this.hoy.setHours(0, 0, 0, 0);
  };
 

  verificarfecha(item: any) {
    if (!item.value.fecha_inicio || !item.value.fecha_final) {
      return "";
    }
  
    // Parsear fechas como locales
    const parseDateLocal = (dateString: string) => {
      const [year, month, day] = dateString.split('-').map(Number);
      return new Date(year, month - 1, day);
    };
  
    const fechaInicio = parseDateLocal(item.value.fecha_inicio);
    const fechaFinal = parseDateLocal(item.value.fecha_final);


    if (fechaInicio > fechaFinal) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser mayor a la fecha final";
    }
  
    if (fechaInicio < this.hoy || fechaFinal < this.hoy) {
      this.error = true;
      return "Error: Las fechas no pueden ser anteriores a hoy";
    }
  
    this.error = false;
    return "";
  }
  
  clickboton() {
    if (this.error) {
      console.error("Error en la validación de fechas");
    } else {
      // Obtén el usuarioid según tu lógica (por ejemplo, a partir del usuario autenticado).
      const usuarioid = 1; // Valor de ejemplo, reemplázalo con el valor real según tu flujo.
      
      this.MandarCarrito.ReservarCarrito(this.carrito, usuarioid)
        .subscribe(
          response => {
            console.log("Respuesta del servidor:", response);
            // Aquí puedes redirigir al usuario o actualizar la interfaz según el resultado.
          },
          err => {
            console.error("Error al reservar carrito:", err);
          }
        );
    }
  }



}
