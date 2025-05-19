import { Component } from '@angular/core';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { SidebardComponent } from '../../sidebard/sidebard.component';

@Component({
  selector: 'app-historial',
  imports: [SidebardComponent],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent {
  contenido : string[] = ["Activo","Aprobado","Pendiente","Rechazado","Finalizado", "Cancelado"];
  item : string = "";

  datos: any[] = [
      {
        fecha_solicitud: "2023-10-01",
        fecha_prestmo: "2023-10-02",
        fecha_devolucion_esperada: "2023-10-03",
        observacion: "Todo en orden",
        estado: "Activo",
        fecha_devolucion: "2023-10-04",
        equipo_nombre: "Laptop",
      },
      {
        fecha_solicitud: "2023-10-05",
        fecha_prestmo: "2023-10-06",
        fecha_devolucion_esperada: "2023-10-07",
        observacion: "Todo en orden",
        estado: "Activo",
        fecha_devolucion: "2023-10-08",
        equipo_nombre: "Laptop NEXUS",
      }
  ];

  constructor(private usuario : UsuarioService) {  }


  itemclick(item : string){
    alert(item);
  }

}
