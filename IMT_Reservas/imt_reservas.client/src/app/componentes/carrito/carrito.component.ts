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

  carrito: Carrito = {};
  constructor(private carritoS: CarritoService) {
    this.carrito  = this.carritoS.obtenercarrito();

  };

  




}
