import { Component, signal, Signal, WritableSignal } from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { FormularioComponent } from '../formulario/formulario.component';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-carrito',
  standalone: true ,
  imports: [CommonModule,FormsModule , MostrarerrorComponent ],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css'
})
export class CarritoComponent {
  private error : boolean = false;

  public errorboton : WritableSignal<boolean> = signal(false);
  public mensajeerror: string = "Datos insertados no validos"; 
  public botonEjecutado: boolean = false;


  hoy : Date= new Date();
  hoystr : string = this.hoy.toISOString().split('T')[0];

 
  fecha_inicio: string = '';
  fecha_final: string = '';

  carrito: Carrito = {};
  


  constructor(private carritoS: CarritoService , private router : Router  , private usuario : UsuarioService) {
    this.carrito  = this.carritoS.obtenercarrito();
    this.hoy.setHours(0, 0, 0, 0);
  };
 
  private parseDateLocal = (dateString: string) => {
      const [year, month, day] = dateString.split('-').map(Number);
      return new Date(year, month - 1, day);
    };

  verificarfecha() {
    if (!this.fecha_inicio || !this.fecha_final) {
      this.error = true;
      return "";
    }

    const fechaInicio = this.parseDateLocal(this.fecha_inicio);
    const fechaFinal = this.parseDateLocal(this.fecha_final);
    const hoyMasUnAnio = new Date(this.hoy);
    hoyMasUnAnio.setFullYear(this.hoy.getFullYear() + 1);

    if (fechaInicio > fechaFinal) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser mayor a la fecha final";
    }
    if (fechaInicio < this.hoy) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser menor a la fecha actual";
    }
    if (hoyMasUnAnio < fechaInicio) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser mayor a un año desde la fecha actual";
    }

    this.error = false;
    return "";
  }
  
  fechamaxima(fechaInicio: string | null): string {
    if (!fechaInicio || fechaInicio==null) return '';
  
    const fecha = new Date(fechaInicio);
    fecha.setFullYear(fecha.getFullYear() + 1);
  

    return fecha.toISOString().split('T')[0];
  }



  clickboton() {
    if ( this.error ) {
      this.errorboton.set(true);
    } 
    else {
      this.validarformulario();
    }
  }

  validarformulario(){
    if(this.usuario.vacio()){
      this.router.navigate(['/Iniciar-Sesion']);
    }
    else{
      this.router.navigate(['/Formulario']);
    }
    
  }

  carritovacio(){
    if (Object.keys(this.carrito).length==0){
      return true; 
    }
    else{
      return false; 
    }
  }

  botonDeshabilitado(): boolean {
    if (this.carritovacio()) return true;
    
    if (!this.fecha_inicio || !this.fecha_final) return true;

    if (this.verificarfecha() !== '') return true;
    
    return false;
  }

  

  generarCantidadesMax(cantidad: number): number[] {
    return Array.from({ length: cantidad }, (_, i) => i + 1);
  }

  cambiarcantidad(key: string, n: number) {
    this.carritoS.editarcantidad(Number(key), n);
  }

  cambiarfechainicio(fecha: string) {
    this.fecha_inicio = fecha;
    // Sincronizar con todos los items del carrito
    Object.values(this.carrito).forEach((item) => {
      item.fecha_inicio = fecha;
    });
  }

  cambiarfechafinal(fecha: string) {
    this.fecha_final = fecha;
    // Sincronizar con todos los items del carrito
    Object.values(this.carrito).forEach((item) => {
      item.fecha_final = fecha;
    });
  }

  abrirCalendario(inputId: string): void {
    const input = document.getElementById(inputId) as HTMLInputElement;
    if (input) {
      try {
        input.showPicker(); // Puede fallar en navegadores viejos
      } catch (error) {
        input.focus(); // fallback: solo hacer focus
      }
    }
  }

}
