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
  selector: 'app-cancelado',
  imports: [CommonModule, VistaPrestamosComponent, MostrarerrorComponent],
  templateUrl: './cancelado.component.html',
  styleUrl: './cancelado.component.css',
})
export class CanceladoComponent extends HistorialBase {
  override estado = 'cancelado';
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
