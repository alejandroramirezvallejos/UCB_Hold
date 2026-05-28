import { Component } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule, DatePipe } from '@angular/common';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-atrasado',
  imports: [CommonModule, DatePipe, VistaPrestamosComponent, AvisoExitoComponent],
  templateUrl: './atrasado.component.html',
  styleUrl: './atrasado.component.css'
})
export class AtrasadoComponent extends HistorialBase {
  override estado: string = 'atrasado';
  constructor(protected override usuario: UsuarioService, protected override prestamoApi: PrestamosAPIService) {
    super(prestamoApi, usuario);
  }
  ngOnInit() {
    this.cargarDatos();
  }
}
