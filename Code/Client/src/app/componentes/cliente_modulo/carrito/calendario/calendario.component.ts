import { CommonModule } from '@angular/common';
import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DisponibilidadService } from '../../../../services/APIS/Disponibilidad/disponibilidad.service';
import { Disponibilidad } from '../../../../models/disponibilidad';
import { Carrito } from '../../../../models/carrito';

@Component({
  selector: 'app-calendario',
  imports: [CommonModule],
  templateUrl: './calendario.component.html',
  styleUrls: ['./calendario.component.css']
})
export class CalendarioComponent {
  @Input() set entradaCarrito(value: Carrito) {
    if(Object.keys(value).length != Object.keys(this.carrito).length){
      const keys : number[] = [];
      for(let key in value){
        keys.push(Number(key));
      }
       this.obtenerDisponibilidad(keys);
    }
    this.carrito = value;
   
  }
  @Input() fechaInicioSeleccionada: WritableSignal<Date | null> = signal(null);
  @Input() fechaFinSeleccionada: WritableSignal<Date | null> = signal(null);
  
  carrito: Carrito = {};

 disponibilidadPorFecha: Map<string,Map<number, number>> = new Map();

 diasDelMes: Date[] = [];

 diaActual: Date = new Date();
 inicio: Date = new Date();

 
 


 constructor(private ApiDisponibilidad : DisponibilidadService){};

  ngOnInit(): void { 
    this.diaActual.setHours(0, 0, 0, 0);
    this.inicio.setHours(0, 0, 0, 0);
    this.generarDiasDelMes();

    console.log(this.disponibilidadPorFecha)
  }

 generarDiasDelMes(): void {
    const primerDia = new Date(this.inicio.getFullYear(), this.inicio.getMonth(), 1);
    const ultimoDia = new Date(this.inicio.getFullYear(), this.inicio.getMonth() + 1, 0);

    this.diasDelMes = [];
    for (let d = new Date(primerDia); d <= ultimoDia; d.setDate(d.getDate() + 1)) {
      this.diasDelMes.push(new Date(d));
    }
  }

  cambiarMes(valor : number){
    this.inicio = new Date(this.inicio.getFullYear(), this.inicio.getMonth() + valor, 1);
    this.generarDiasDelMes();
  }

  obtenerDisponibilidad(keys : number[]){
    this.ApiDisponibilidad.obtenerDisponibilidad(new Date(), new Date(new Date().getFullYear() + 1, new Date().getMonth(), new Date().getDate()), keys).subscribe({
      next: (data : Disponibilidad[]) => {
        this.disponibilidadPorFecha.clear();
        data.forEach(item => {
          if(item.Fecha){
            const fecha : string = this.toLocalISOString(new Date(item.Fecha));
            if (!this.disponibilidadPorFecha.has(fecha)) {
              this.disponibilidadPorFecha.set(fecha, new Map());
            }
            this.disponibilidadPorFecha.get(fecha)!.set(item.IdGrupoEquipo, item.CantidadDisponible);
          }
        });

      },
      error: (error) => {
        console.error('Error al obtener la disponibilidad:', error);
      }
    })
  }




  selecionarFecha(fecha: Date): void {

    if (!this.fechaInicioSeleccionada() || (this.fechaInicioSeleccionada() && this.fechaFinSeleccionada())) {
      this.fechaInicioSeleccionada.set(new Date(fecha));
      this.fechaFinSeleccionada.set(null);
    }
    else {
      if (fecha.getTime() < this.fechaInicioSeleccionada()!.getTime()) {
        this.fechaFinSeleccionada.set(new Date(this.fechaInicioSeleccionada()!));
        this.fechaInicioSeleccionada.set(new Date(fecha));
      } 
      else {
        this.fechaFinSeleccionada.set(new Date(fecha));
      }
    }
    

  }

  esFechaSeleccionada(fecha: Date): boolean {
    if (!this.fechaInicioSeleccionada()) return false;
    const inicio : number= this.fechaInicioSeleccionada()!.getTime();
    const fin: number  = this.fechaFinSeleccionada() ? this.fechaFinSeleccionada()!.getTime() : inicio;
    return fecha.getTime() >= inicio && fecha.getTime() <= fin;
  }


  obtenerFechaKey(date: Date): string {
    return this.toLocalISOString(date);
  }


// revisar 
  estaOcupado(dia : Date): boolean {
    const fechaKey = this.obtenerFechaKey(dia);
    if(this.disponibilidadPorFecha.has(fechaKey)){
      for(let key in this.carrito){
         if( ( this.disponibilidadPorFecha.get(fechaKey)?.get(Number(key))  ?? 0 ) < this.carrito[key].cantidad ){
          console.log("ocupado en fecha: " + fechaKey + " para el grupo de equipo: " + key);
          return true; 
         }
      }
      return false; 
    }
    else{
        return true; 
    }

  }

  private toLocalISOString(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().split('T')[0];
  }

}
