import { Component, signal, WritableSignal } from '@angular/core';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { Prestamos } from '../../../../models/admin/Prestamos';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule, DatePipe } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';

import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { Aviso } from '../../../pantallas_avisos/aviso/aviso.component';

@Component({
  selector: 'app-activo',
  imports: [CommonModule, DatePipe , Aviso,VistaPrestamosComponent],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css'
})
export class ActivoComponent extends HistorialBase {

  
  override estado: string = 'activo';
  
  aviso : WritableSignal<boolean> = signal(false);

  constructor(   protected override usuario : UsuarioService ,  protected override prestamoApi : PrestamosAPIService){
    super(prestamoApi, usuario);
  }; 


  ngOnInit() {
    this.cargarDatos();
  }

 

  avisoDevolver(item : Prestamos) {
    this.itemSeleccionado = item;
    this.aviso.set(true);
  }

  

  finalizado() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'finalizado').subscribe({
      next: (response) => {
        this.cargarDatos();

        this.itemSeleccionado = null;
      }, 
      error: (error) => {
        this.mensajeerror = "Error al finalizar el prestamo, intente mas tarde";
        console.error( error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }




}
