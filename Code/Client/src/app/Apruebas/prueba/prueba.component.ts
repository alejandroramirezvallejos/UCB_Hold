import { Component } from '@angular/core';
import { CalendarioComponent } from '../../componentes/cliente_modulo/carrito/calendario/calendario.component';
import { Disponibilidad } from '../../models/disponibilidad';
import { DisponibilidadService } from '../../services/APIS/Disponibilidad/disponibilidad.service';

@Component({
  selector: 'app-prueba',
  imports: [CalendarioComponent],
  templateUrl: './prueba.component.html',
  styleUrls: ['./prueba.component.css']
})
export class PruebaComponent {
  fechasdisponibles : Disponibilidad[] = [];


  constructor(private SDisponibilidad : DisponibilidadService) { }

  ngOnInit(){
    this.obtenerFechasDisponibles();
  }

  obtenerFechasDisponibles(){
    this.SDisponibilidad.obtenerDisponibilidad(new Date(), new Date(), [32]).subscribe(data=>{
      this.fechasdisponibles = data;
      console.log(this.fechasdisponibles);
    });
  }

  


}
