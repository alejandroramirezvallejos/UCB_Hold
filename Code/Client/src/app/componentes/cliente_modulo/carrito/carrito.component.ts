import { Component, signal, Signal, WritableSignal } from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { FormularioComponent } from '../formulario/formulario.component';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../pantallas_avisos/aviso/aviso.component';
import { PrestamosAPIService } from '../../../services/APIS/prestamo/prestamos-api.service';
import { AvisoExitoComponent } from '../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { PantallaCargaComponent } from '../../pantallas_avisos/pantalla-carga/pantalla-carga.component';
import { CalendarioComponent } from './calendario/calendario.component';

@Component({
  selector: 'app-carrito',
  standalone: true ,
  imports: [CommonModule,FormsModule , MostrarerrorComponent , Aviso , AvisoExitoComponent , PantallaCargaComponent , CalendarioComponent],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css'
})
export class CarritoComponent {
  private error : boolean = false;

  public errorboton : WritableSignal<boolean> = signal(false);
  public mensajeerror: string = "Datos insertados no validos"; 

  aviso : WritableSignal<boolean> = signal(false);

  exito : WritableSignal<boolean> = signal(false);

  cargando : boolean = false; 



  public readonly precioMax : number = 2000;


  hoy : Date= new Date();
  hoystr : string = this.toLocalISOString(this.hoy);


  fecha_inicio: WritableSignal<Date | null> = signal(null);
  fecha_final: WritableSignal<Date | null> = signal(null);

  carrito: Carrito = {};
  



  constructor(public carritoS: CarritoService , private router : Router  , private usuario : UsuarioService , private Sprestamo : PrestamosAPIService) {
    this.carrito  = this.carritoS.obtenercarrito();
    this.hoy.setHours(0, 0, 0, 0);
  };


 
  private parseDateLocal = (dateString: string) => {
      const [year, month, day] = dateString.split('-').map(Number);
      return new Date(year, month - 1, day);
    };


  get fechaInicioStr(): string {
    const fecha = this.fecha_inicio();
    return fecha ? this.toLocalISOString(fecha) : '';
  }

  set fechaInicioStr(value: string) {
    this.fecha_inicio.set(value ? this.parseDateLocal(value) : null);
  }

  get fechaFinalStr(): string {
    const fecha = this.fecha_final();
    return fecha ? this.toLocalISOString(fecha) : '';
  }

  set fechaFinalStr(value: string) {
    this.fecha_final.set(value ? this.parseDateLocal(value) : null);
  }


  verificarfecha() {
    const fechaInicio = this.fecha_inicio();
    const fechaFinal = this.fecha_final();
    if (!fechaInicio || !fechaFinal) {
      this.error = true;
      return "";
    }

   
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
  
  fechamaxima(fechaInicio: Date | null): string {
    if (!fechaInicio) return '';
    const fecha = new Date(fechaInicio);
    fecha.setFullYear(fecha.getFullYear() + 1);
    return this.toLocalISOString(fecha);
  }



  clickboton() {
    if (this.error) {
      this.errorboton.set(true);
    } 
    else if(this.usuario.vacio()){
        this.router.navigate(['/Iniciar-Sesion']);
    }
    else if(this.carritoS.preciototal()<this.precioMax) {
        this.aviso.set(true);
    }
    else{
        this.cambiarfechainicio(this.fechaInicioStr);
        this.cambiarfechafinal(this.fechaFinalStr);
       this.router.navigate(['/Formulario']);
    }
  }

  realizarPrestamo(){
    this.cargando = true;
    this.cambiarfechainicio(this.fechaInicioStr);
    this.cambiarfechafinal(this.fechaFinalStr);
    console.log('Carrito:', this.carrito);
    this.Sprestamo.crearPrestamo(this.carrito , this.usuario.obtenercarnet() , null).subscribe({
      next: () => {
        this.carritoS.vaciarcarrito();
        this.exito.set(true);
        this.cargando = false;
      }
      , error: (err) => {
        this.errorboton.set(true);
        this.mensajeerror = "Error al realizar el prestamo intente nuevamente mas tarde" ;
        console.error('Error al crear el prestamo:', err);
        this.cargando = false;
      }
    })
  }

  redirigirHome(){
    this.router.navigate(['/home']);
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
    
    if (!this.fecha_inicio() || !this.fecha_final()) return true;

    if (this.verificarfecha() !== '') return true;
    
    return false;
  }

  

  generarCantidadesMax(cantidad: number): number[] {
    return Array.from({ length: cantidad }, (_, i) => i + 1);
  }

  cambiarcantidad(key: string, n: number) {
    this.carritoS.editarcantidad(Number(key), Number(n));
  }

   cambiarfechainicio(fecha: string) {
    this.fecha_inicio.set(this.parseDateLocal(fecha));
    // Sincronizar con todos los items del carrito
    Object.values(this.carrito).forEach((item) => {
      item.fecha_inicio = fecha;
    });
  }

  cambiarfechafinal(fecha: string) {
    this.fecha_final.set(this.parseDateLocal(fecha));
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



  public toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().split('T')[0];
  }



}
