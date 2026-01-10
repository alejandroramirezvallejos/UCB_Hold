import { Component } from '@angular/core';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { ActivoComponent } from './activo/activo.component';
import { AprobadoComponent } from './aprobado/aprobado.component';
import { CanceladoComponent } from './cancelado/cancelado.component';
import { FinalizadoComponent } from './finalizado/finalizado.component';
import { PendienteComponent } from './pendiente/pendiente.component';
import { RechazadoComponent } from './rechazado/rechazado.component';

@Component({
  selector: 'app-historial',
  imports: [ActivoComponent , AprobadoComponent, CanceladoComponent , FinalizadoComponent , PendienteComponent , RechazadoComponent],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent {
  contenido : string[] = ["Activo","Aprobado","Pendiente","Rechazado","Finalizado", "Cancelado"];
  item : string = "Activo";
  isOpen: boolean = false;
  isHovered: boolean = false;

  constructor(private usuario : UsuarioService) {  }


  itemclick(item : string){
    this.item=item;
    this.isOpen = false;
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  onMouseEnter() {
    this.isHovered = true;
  }

  onMouseLeave() {
    this.isHovered = false;
  }
}
