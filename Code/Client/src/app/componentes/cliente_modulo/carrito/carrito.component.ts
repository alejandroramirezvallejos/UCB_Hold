import { Component, signal, Signal, WritableSignal } from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MostrarerrorComponent } from '../../mostrarerror/mostrarerror.component';
import { FormularioComponent } from '../formulario/formulario.component';
import { Router } from '@angular/router';
import { Carrito } from '../../../models/carrito';

@Component({
  selector: 'app-carrito',
  standalone: true,
  imports: [CommonModule, FormsModule, MostrarerrorComponent],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css',
})
export class CarritoComponent {
  private error: boolean = false;
  // 0 es falso 1 es true para errorboton
  public errorboton: WritableSignal<number> = signal(2);
  public mensajeerror: string = 'Datos insertados no validos';
  public botonEjecutado: boolean = false;
  hoy: string;
  cantidades = Array.from({ length: 11 }, (_, i) => i);
  carrito: Carrito = {};

  constructor(private carritoS: CarritoService, private router: Router) {
    this.carrito = this.carritoS.obtenercarrito();
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    this.hoy = today.toISOString().slice(0, 10);

    this.onInicioChange(this.hoy);
    this.onFinChange(this.hoy);
  }

  private parseDateLocal = (dateString: string) => {
    const [year, month, day] = dateString.split('-').map(Number);
    return new Date(year, month - 1, day);
  };

  onCantidadChange(key: string, n: number) {
    this.carritoS.editarcantidad(Number(key), n);
  }

  onInicioChange(newDate: string) {
    Object.values(this.carrito).forEach((item) => {
      item.fecha_inicio = newDate;
    });
  }

  onFinChange(newDate: string) {
    Object.values(this.carrito).forEach((item) => {
      item.fecha_final = newDate;
    });
  }

  verificarfecha(item: any) {
    if (!item.value.fecha_inicio || !item.value.fecha_final) {
      this.error = true;
      return '';
    }

    const fechaInicio = this.parseDateLocal(item.value.fecha_inicio);
    const fechaFinal = this.parseDateLocal(item.value.fecha_final);

    if (fechaInicio > fechaFinal) {
      this.error = true;
      return 'Error: La fecha de inicio no puede ser mayor a la fecha final';
    }

    this.error = false;
    return '';
  }

  clickboton() {
    if (this.error) {
      this.errorboton.set(1);
    } else {
      this.errorboton.set(0);
    }
  }

  formulario() {
    this.router.navigate(['/Formulario']);
  }
}
