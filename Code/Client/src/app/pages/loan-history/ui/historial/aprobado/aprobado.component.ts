import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoDto } from '@entities/admin';
import { PrestamosAPIService, VistaPrestamosComponent } from '@entities/loan';
import { UsuarioService } from '@entities/user';
import { extractErrorMessage } from '@shared/lib/error';
import { Aviso, AvisoExitoComponent } from '@shared/ui';
import { HistorialBase } from '../base/historial-base';
@Component({
  selector: 'app-aprobado',
  standalone: true,
  imports: [CommonModule, Aviso, VistaPrestamosComponent, AvisoExitoComponent],
  templateUrl: './aprobado.component.html',
  styleUrl: './aprobado.component.css',
})
export class AprobadoComponent extends HistorialBase {
  override estado: string = 'aprobado';
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

  avisocancelarf(item: PrestamoDto) {
    this.avisocancelar.set(!this.avisocancelar());
    this.itemSeleccionado = item;
  }

  cancelar() {
    this.prestamoApi
      .cambiarEstadoPrestamo(this.itemSeleccionado!.Id, 'cancelado')
      .subscribe({
        next: (_response) => {
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
