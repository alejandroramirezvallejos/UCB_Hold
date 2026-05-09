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
  imports: [CommonModule, DatePipe , Aviso,VistaPrestamosComponent , AvisoExitoComponent],
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
  avisoDevolver(item : PrestamoDto) {
    this.itemSeleccionado = item;
    this.aviso.set(true);
  }
  finalizado() {
    this.prestamoApi.cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'finalizado').subscribe({
      next: (response) => {
        this.cargarDatos();
        this.itemSeleccionado = null;
        this.mensajeexito = "Préstamo devuelto con éxito , ahora pasa a Finalizado";
        this.exito.set(true);
      }, 
      error: (error) => {
        const msg = error.error?.errors?.[0] || error.error?.message || error.message || 'Error desconocido';
        this.mensajeerror = `Error al finalizar el préstamo: ${msg}`;
        console.error(msg);
        this.error.set(true);
      }
    });
  }
}
