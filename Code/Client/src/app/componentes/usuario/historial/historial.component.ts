import { Component } from '@angular/core';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { SidebardComponent } from '../../sidebard/sidebard.component';
import { ActivoComponent } from './activo/activo.component';
import { AprobadoComponent } from './aprobado/aprobado.component';
import { CanceladoComponent } from './cancelado/cancelado.component';
import { FinalizadoComponent } from './finalizado/finalizado.component';
import { PendienteComponent } from './pendiente/pendiente.component';
import { RechazadoComponent } from './rechazado/rechazado.component';

@Component({
  selector: 'app-historial',
  imports: [SidebardComponent , ActivoComponent , AprobadoComponent, CanceladoComponent , FinalizadoComponent , PendienteComponent , RechazadoComponent],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent {
  contenido : string[] = ["Activo","Aprobado","Pendiente","Rechazado","Finalizado", "Cancelado"];
  item : string = "Activo";

  constructor(private usuario : UsuarioService) {  }


  itemclick(item : string){
    this.item=item; 
  }

}
