import { Component, signal, WritableSignal } from '@angular/core';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';

import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { Aviso } from '../../../pantallas_avisos/aviso/aviso.component';

@Component({
  selector: 'app-aprobado',
  standalone: true,
  imports: [CommonModule, Aviso , VistaPrestamosComponent],
  templateUrl: './aprobado.component.html',
  styleUrl: './aprobado.component.css'
})
export class AprobadoComponent extends HistorialBase {

  override estado: string = 'aprobado';

  avisocancelar : WritableSignal<boolean> = signal(false);
  avisoaprobar : WritableSignal<boolean> = signal(false);


  constructor( protected override usuario : UsuarioService ,  protected override prestamoApi : PrestamosAPIService)
  {super(prestamoApi, usuario);}; 


  ngOnInit() {
    this.cargarDatos();
  }

 validarFechaRecogida(item: any): boolean {
   return item.value.datosgrupo.FechaPrestamoEsperada && item.value.datosgrupo.FechaPrestamoEsperada > new Date();
 }

  avisocancelarf(item : Prestamos) {
    this.avisocancelar.set(!this.avisocancelar());
    this.itemSeleccionado = item;
  }

  avisorecogerf(item : Prestamos){
     this.avisoaprobar.set(!this.avisoaprobar());
     this.itemSeleccionado = item;

  }

  recoger() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'activo').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.avisoaprobar.set(false);
      }, 
      error: (error) => {
        this.mensajeerror = "Error al recoger el prestamo, intente mas tarde";
        console.error( error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cancelar() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'cancelado').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.avisocancelar.set(false);
      }, 
      error: (error) => {
        this.mensajeerror = "Error al cancelar el prestamo, intente mas tarde";
        console.error( error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }


}
