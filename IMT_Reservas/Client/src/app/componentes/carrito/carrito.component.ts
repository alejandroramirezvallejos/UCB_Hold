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
  hoy : Date= new Date();
 

  carrito: Carrito = {};
  constructor(private carritoS: CarritoService) {
    this.carrito  = this.carritoS.obtenercarrito();
    this.hoy.setHours(0, 0, 0, 0);
  };
 

  verificarfecha(item: any) {
    if (item.value.fecha_inicio==null || !item.value.fecha_final==null) 
    {
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
  
  clickboton(){
    if(this.error==true){
      console.error("error");
    }
    else{
      console.log("boton clickado");
    }
    
  }



}
