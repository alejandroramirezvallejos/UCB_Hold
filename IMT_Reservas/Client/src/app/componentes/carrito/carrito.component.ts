import { Component } from '@angular/core';
import { CarritoService } from '../../services/carrito/carrito.service';
import {Carrito } from '../../models/carrito'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-carrito',
  standalone: true ,
  imports: [CommonModule,FormsModule],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css'
})
export class CarritoComponent {
  private error : boolean = false;
  hoy : string;
 

  carrito: Carrito = {};
  constructor(private carritoS: CarritoService  ) {
    this.carrito  = this.carritoS.obtenercarrito();
    const fechaLocal = new Date();
    const año = fechaLocal.getFullYear();
    const mes = (fechaLocal.getMonth() + 1).toString().padStart(2, '0');
    const día = fechaLocal.getDate().toString().padStart(2, '0');
    const fechaISO = `${año}-${mes}-${día}`;

    this.hoy=fechaISO;
  };

  // Parsear fechas como locales
  private parseDateLocal = (dateString: string) => {
      const [year, month, day] = dateString.split('-').map(Number);
      return new Date(year, month - 1, day);
    };

  verificarfecha(item: any) {
    if (!item.value.fecha_inicio || !item.value.fecha_final) {
      return "";
    }
  
    const fechaInicio = this.parseDateLocal(item.value.fecha_inicio);
    const fechaFinal = this.parseDateLocal(item.value.fecha_final);

    if (fechaInicio > fechaFinal) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser mayor a la fecha final";
    }
  
  
    this.error = false;
    return "";
  }
  
  clickboton() {
    if (this.error) {
      console.error("Error en la validación de fechas");
    } else {
    
    }
  }



}
