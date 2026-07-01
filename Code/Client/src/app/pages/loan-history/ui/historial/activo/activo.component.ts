import { Component, signal, WritableSignal } from '@angular/core';
import { UsuarioService } from '@entities/user';
import { PrestamoDto } from '@entities/admin';
import { PrestamosAPIService } from '@entities/loan';
import { CommonModule, DatePipe } from '@angular/common';
import { PrestamoAgrupados } from '@entities/loan';
import { HistorialBase } from '../BASE/HistorialBase';
import { VistaPrestamosComponent } from '@entities/loan';
import { Aviso } from '@shared/ui';
import { AvisoExitoComponent } from '@shared/ui';
@Component({
  selector: 'app-activo',
  imports: [
    CommonModule,
    DatePipe,
    VistaPrestamosComponent,
    AvisoExitoComponent,
  ],
  templateUrl: './activo.component.html',
  styleUrl: './activo.component.css',
})
export class ActivoComponent extends HistorialBase {
  override estado: string = 'activo';
  constructor(
    protected override usuario: UsuarioService,
    protected override prestamoApi: PrestamosAPIService,
  ) {
    super(prestamoApi, usuario);
  }
  ngOnInit() {
    this.cargarDatos();
  }
}
