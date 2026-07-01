import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoAgrupados } from '@entities/loan';
import { UsuarioService } from '@entities/user';
import { PrestamosAPIService } from '@entities/loan';
import { PrestamoDto } from '@entities/admin';
import { CommonModule } from '@angular/common';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '@entities/loan';
import { Aviso } from '@shared/ui';
import { MostrarerrorComponent } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
@Component({
  selector: 'app-pendiente',
  imports: [
    CommonModule,
    Aviso,
    VistaPrestamosComponent,
    MostrarerrorComponent,
    AvisoExitoComponent,
  ],
  templateUrl: './pendiente.component.html',
  styleUrl: './pendiente.component.css',
})
export class PendienteComponent extends HistorialBase {
  override estado: string = 'pendiente';
  avisocancelar: WritableSignal<boolean> = signal(false);
  constructor(
    protected override usuario: UsuarioService,
    protected override prestamoApi: PrestamosAPIService,
  ) {
    super(prestamoApi, usuario);
  }
  ngOnInit() {
    this.cargarDatos();
  }
  aviso(item: PrestamoDto) {
    this.itemSeleccionado = item;
    this.avisocancelar.set(!this.avisocancelar());
  }
  abrirAvisoCancelacion(event: Event, item: PrestamoDto): void {
    event.stopPropagation();
    this.aviso(item);
  }
  cancelar() {
    this.prestamoApi
      .cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'cancelado')
      .subscribe({
        next: (response) => {
          this.cargarDatos();
          this.itemSeleccionado = null;
          this.avisocancelar.set(false);
          this.mensajeexito =
            'Préstamo cancelado con éxito , ahora pasa a Cancelado';
          this.exito.set(true);
        },
        error: (error) => {
          const msg = extractErrorMessage(error);
          this.mensajeerror = `Error al cancelar el préstamo: ${msg}`;
          this.error.set(true);
        },
      });
  }
}
