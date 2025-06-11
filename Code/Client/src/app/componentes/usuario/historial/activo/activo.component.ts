import { Component } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';

@Component({
  selector: 'app-activo',
  imports: [],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css'
})
export class ActivoComponent {
 datos: any[] = [
      {
        fecha_solicitud: "2023-10-01",
        fecha_prestamo: "2023-10-02",
        fecha_devolucion_esperada: "2023-10-03",
        observacion: "Todo en orden",
        estado: "Activo",
        fecha_devolucion: "2023-10-04",
        equipo_nombre: "Laptop",
      },
      {
        fecha_solicitud: "2023-10-05",
        fecha_prestamo: "2023-10-06",
        fecha_devolucion_esperada: "2023-10-07",
        observacion: "Todo en orden",
        estado: "Activo",
        fecha_devolucion: "2023-10-08",
        equipo_nombre: "Laptop NEXUS",
      }
  ];


constructor(usuario : UsuarioService){}; 


}
