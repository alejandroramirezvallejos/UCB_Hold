import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';
import { Aviso } from '../aviso/aviso.component';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';

@Component({
  selector: 'app-pendiente',
  imports: [CommonModule , Aviso , VistaPrestamosComponent],
  templateUrl: './pendiente.component.html',
  styleUrl: './pendiente.component.css'
})
export class PendienteComponent extends HistorialBase {


  override estado: string = 'pendiente';

 
  avisocancelar : WritableSignal<boolean> = signal(false);

  constructor( protected override usuario : UsuarioService ,  protected override prestamoApi : PrestamosAPIService)
  {super(prestamoApi, usuario);}; 


  ngOnInit() {
    this.cargarDatos();
  }

  
  aviso(item : Prestamos) {
    this.itemSeleccionado = item;
    this.avisocancelar.set(!this.avisocancelar());

  }


  cancelar() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'cancelado').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.avisocancelar.set(false);
      }, 
      error: (error) => {
        alert( error.error.error + ': ' + error.error.mensaje);
      }
    });
  }


}
