import { Component } from '@angular/core';
import { PrestamoAgrupados } from '@entities/loan';
import { UsuarioService } from '@entities/user';
import { PrestamosAPIService } from '@entities/loan';
import { PrestamoDto } from '@entities/admin';
import { CommonModule } from '@angular/common';
import { HistorialBase } from '../base/historial-base';
import { VistaPrestamosComponent } from '@entities/loan';
import { MostrarerrorComponent } from '@shared/ui';
@Component({
  selector: 'app-finalizado',
  imports: [CommonModule, VistaPrestamosComponent, MostrarerrorComponent],
  templateUrl: './finalizado.component.html',
  styleUrl: './finalizado.component.css',
})
export class FinalizadoComponent extends HistorialBase {
  override estado: string = 'finalizado';
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
