import { Component, OnDestroy, OnInit } from '@angular/core';
import { UsuarioService } from '../../../services/usuario/usuario.service';
import { ActivoComponent } from './activo/activo.component';
import { AprobadoComponent } from './aprobado/aprobado.component';
import { CanceladoComponent } from './cancelado/cancelado.component';
import { FinalizadoComponent } from './finalizado/finalizado.component';
import { PendienteComponent } from './pendiente/pendiente.component';
import { RechazadoComponent } from './rechazado/rechazado.component';
@Component({
  selector: 'app-historial',
  imports: [ActivoComponent, AprobadoComponent, CanceladoComponent, FinalizadoComponent, PendienteComponent, RechazadoComponent],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css'
})
export class HistorialComponent implements OnInit, OnDestroy {
  contenido: string[] = ["Activo", "Aprobado", "Pendiente", "Rechazado", "Finalizado", "Cancelado"];
  item: string = "Activo";
  isOpen: boolean = false;
  isHovered: boolean = false;
  show: boolean = true;
  private pollInterval: ReturnType<typeof setInterval> | null = null;

  constructor(private usuario: UsuarioService) { }

  ngOnInit() {
    this.pollInterval = setInterval(() => this.recargar(), 30000);
  }

  ngOnDestroy() {
    if (this.pollInterval) clearInterval(this.pollInterval);
  }

  itemclick(item: string) {
    this.item = item;
    this.isOpen = false;
    this.recargar();
  }

  private recargar() {
    this.show = false;
    setTimeout(() => { this.show = true; }, 0);
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }
  onMouseEnter() {
    this.isHovered = true;
  }
  onMouseLeave() {
    this.isHovered = false;
  }
}
