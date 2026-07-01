import { Component } from '@angular/core';
import { UsuarioService } from '@entities/user';
import { PrestamosAPIService } from '@entities/loan';
import { CommonModule, DatePipe } from '@angular/common';
import { HistorialBase } from '../base/historial-base';
import { VistaPrestamosComponent } from '@entities/loan';
import { AvisoExitoComponent } from '@shared/ui';

@Component({
  selector: 'app-atrasado',
  imports: [
    CommonModule,
    DatePipe,
    VistaPrestamosComponent,
    AvisoExitoComponent,
  ],
  templateUrl: './atrasado.component.html',
  styleUrl: './atrasado.component.css',
})
export class AtrasadoComponent extends HistorialBase {
  override estado: string = 'atrasado';
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
