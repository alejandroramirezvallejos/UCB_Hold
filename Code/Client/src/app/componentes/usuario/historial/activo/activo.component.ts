import { Component, signal, WritableSignal } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamoDto } from '../../../../models/admin/Prestamos';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule, DatePipe } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { Aviso } from '../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';
@Component({
  selector: 'app-activo',
  imports: [CommonModule, DatePipe ,VistaPrestamosComponent , AvisoExitoComponent],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css'
})
export class ActivoComponent extends HistorialBase {
  override estado: string = 'activo';
  constructor(   protected override usuario : UsuarioService ,  protected override prestamoApi : PrestamosAPIService){
    super(prestamoApi, usuario);
  }; 
  ngOnInit() {
    this.cargarDatos();
  }
}
