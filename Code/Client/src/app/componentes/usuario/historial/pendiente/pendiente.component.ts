import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { CommonModule } from '@angular/common';

import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { Aviso } from '../../../pantallas_avisos/aviso/aviso.component';
import { MostrarerrorComponent } from '../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-pendiente',
  imports: [CommonModule , Aviso , VistaPrestamosComponent, MostrarerrorComponent, AvisoExitoComponent],
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
        this.mensajeexito = "Préstamo cancelado con éxito , ahora pasa a Cancelado";
        this.exito.set(true);
      }, 
      error: (error) => {
        this.mensajeerror = "Error al cancelar el prestamo, intente mas tarde";
        console.error( error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }


}
