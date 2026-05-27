import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoDto } from '../../../../models/admin/Prestamos';
import { UsuarioService } from '../../../../services/usuario/usuario.service';
import { PrestamosAPIService } from '../../../../services/APIS/prestamo/prestamos-api.service';
import { CommonModule } from '@angular/common';
import { PrestamoAgrupados } from '../../../../models/PrestamoAgrupados';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '../../../vista-prestamos/vista-prestamos.component';
import { Aviso } from '../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../pantallas_avisos/aviso-exito/aviso-exito.component';
@Component({
  selector: 'app-aprobado',
  standalone: true,
  imports: [CommonModule, Aviso , VistaPrestamosComponent, AvisoExitoComponent],
  templateUrl: './aprobado.component.html',
  styleUrl: './aprobado.component.css'
})
export class AprobadoComponent extends HistorialBase {
  override estado: string = 'aprobado';
  avisocancelar : WritableSignal<boolean> = signal(false);
  constructor( protected override usuario : UsuarioService ,  protected override prestamoApi : PrestamosAPIService)
  {
    super(prestamoApi, usuario);
  }; 


  ngOnInit() {
    this.cargarDatos();
  }


  avisocancelarf(item : PrestamoDto) {
    this.avisocancelar.set(!this.avisocancelar());
    this.itemSeleccionado = item;
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
        const msg = error.error?.errors?.[0] || error.error?.message || error.message || 'Error desconocido';
        this.mensajeerror = `Error al cancelar el préstamo: ${msg}`;
        console.error(msg);
        this.error.set(true);
      }
    });
  }
}
