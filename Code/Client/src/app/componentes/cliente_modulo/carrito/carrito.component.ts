import { Component, signal, Signal, WritableSignal } from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MostrarerrorComponent } from '../../mostrarerror/mostrarerror.component';
import { FormularioComponent } from '../formulario/formulario.component';
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';

@Component({
  selector: 'app-carrito',
  standalone: true ,
  imports: [CommonModule,FormsModule , MostrarerrorComponent ],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css'
})
export class CarritoComponent {
  private error : boolean = false;
  // 0 es falso 1 es true para errorboton
  public errorboton : WritableSignal<number> = signal(2);
  public mensajeerror: string = "Datos insertados no validos"; 
  public botonEjecutado: boolean = false;
  public cantidades = Array.from({ length: 11 }, (_, i) => i);

  hoy : Date= new Date();
  hoystr : string = this.hoy.toISOString().split('T')[0];

  carrito: Carrito = {};
  constructor(private carritoS: CarritoService , private router : Router  , private usuario : UsuarioService) {
    this.carrito  = this.carritoS.obtenercarrito();
    this.hoy.setHours(0, 0, 0, 0);
  };
 
  private parseDateLocal = (dateString: string) => {
      const [year, month, day] = dateString.split('-').map(Number);
      return new Date(year, month - 1, day);
    };

  verificarfecha(item: any) {
    if (!item.value.fecha_inicio || !item.value.fecha_final  ) {
      this.error=true;
      return "";
    }

  
 
  
    const fechaInicio = this.parseDateLocal(item.value.fecha_inicio);
    const fechaFinal = this.parseDateLocal(item.value.fecha_final);
    const hoyMasUnAnio = new Date(this.hoy);
    hoyMasUnAnio.setFullYear(this.hoy.getFullYear() + 1);

    if (fechaInicio > fechaFinal) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser mayor a la fecha final";
    }
    if( fechaInicio < this.hoy) {
      this.error = true;
      return "Error: La fecha de inicio no puede ser menor a la fecha actual";
    }
    if(hoyMasUnAnio  <fechaInicio ){
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
      this.errorboton.set(1);
    } 
    else {
      
      this.errorboton.set(0);
      

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

  



  cambiarcantidad(key: string, n: number) {
    this.carritoS.editarcantidad(Number(key), n);
  }

  cambiarfechainicio(fecha: string) {
    Object.values(this.carrito).forEach((item) => {
      item.fecha_inicio = fecha;
    });
  }

  cambiarfechafinal(fecha: string) {
    Object.values(this.carrito).forEach((item) => {
      item.fecha_final = fecha;
    });
  }

}
