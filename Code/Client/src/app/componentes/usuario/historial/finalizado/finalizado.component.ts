import { Component } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';
import { HistorialBase } from '../BASE/HistorialBase';

@Component({
  selector: 'app-finalizado',
  imports: [CommonModule],
  templateUrl: './finalizado.component.html',
  styleUrl: './finalizado.component.css'
})
export class FinalizadoComponent  extends HistorialBase{


  override estado: string = 'finalizado';

  constructor( protected override usuario : UsuarioService , protected override prestamoApi : PrestamosAPIService)
  {super(prestamoApi, usuario);}; 


  ngOnInit() {
    this.cargarDatos();
  }


}
