import { Component } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';

@Component({
  selector: 'app-cancelado',
  imports: [CommonModule , VistaPrestamosComponent],
  templateUrl: './cancelado.component.html',
  styleUrl: './cancelado.component.css'
})
export class CanceladoComponent  extends HistorialBase{


  override estado = 'cancelado';

  constructor( protected override usuario : UsuarioService , protected override prestamoApi : PrestamosAPIService)
  {super(prestamoApi, usuario);}; 


  ngOnInit() {
    this.cargarDatos();
  }


 

}
