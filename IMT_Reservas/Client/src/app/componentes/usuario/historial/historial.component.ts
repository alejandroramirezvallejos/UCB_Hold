import { Component } from '@angular/core';
import { UsuarioService } from '../../../services/usuario/usuario.service';

@Component({
  selector: 'app-historial',
  imports: [],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent {


  constructor(private usuario : UsuarioService) {  }

  


}
