import { Component, signal, WritableSignal } from '@angular/core';
import { PrestamoDto } from '@entities/admin';
import { UsuarioService } from '@entities/user';
import { PrestamosAPIService } from '@entities/loan';
import { CommonModule } from '@angular/common';
import { PrestamoAgrupados } from '@entities/loan';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '@entities/loan';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
import { extractErrorMessage } from '@shared/lib/error';
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
