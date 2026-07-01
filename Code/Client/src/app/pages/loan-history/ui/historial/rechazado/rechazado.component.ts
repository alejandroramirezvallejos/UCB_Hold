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
  selector: 'app-rechazado',
  imports: [CommonModule, VistaPrestamosComponent, MostrarerrorComponent],
  templateUrl: './rechazado.component.html',
  styleUrl: './rechazado.component.css',
})
export class RechazadoComponent extends HistorialBase {
  override estado: string = 'rechazado';
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
