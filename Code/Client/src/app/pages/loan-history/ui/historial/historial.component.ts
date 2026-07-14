import {
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlatpickrDirective } from '@shared/lib/directives';
import flatpickr from 'flatpickr';
import { ActivoComponent } from './activo/activo.component';
import { AprobadoComponent } from './aprobado/aprobado.component';
import { AtrasadoComponent } from './atrasado/atrasado.component';
import { CanceladoComponent } from './cancelado/cancelado.component';
import { FinalizadoComponent } from './finalizado/finalizado.component';
import { PendienteComponent } from './pendiente/pendiente.component';
import { RechazadoComponent } from './rechazado/rechazado.component';
@Component({
  selector: 'app-historial',
  imports: [
    ActivoComponent,
    AprobadoComponent,
    AtrasadoComponent,
    CanceladoComponent,
    FinalizadoComponent,
    PendienteComponent,
    RechazadoComponent,
    FormsModule,
    FlatpickrDirective,
  ],
  templateUrl: './historial.component.html',
  styleUrl: './historial.component.css',
})
export class HistorialComponent implements OnInit, OnDestroy {
  contenido: string[] = [
    'Activo',
    'Aprobado',
    'Pendiente',
    'Rechazado',
    'Finalizado',
    'Cancelado',
    'Atrasado',
  ];
  item: string = 'Activo';
  isOpen: boolean = false;
  isHovered: boolean = false;
  show: boolean = true;
  filtroTexto: string = '';
  fechaDesde: string = '';
  fechaHasta: string = '';
  private pollInterval: ReturnType<typeof setInterval> | null = null;

  constructor(private el: ElementRef) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.isOpen && !this.el.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }

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
    setTimeout(() => {
      this.show = true;
    }, 0);
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  onFechaDesde(dates: Date[]) {
    this.fechaDesde = dates[0] ? dates[0].toISOString().split('T')[0] : '';
  }

  onFechaHasta(dates: Date[]) {
    this.fechaHasta = dates[0] ? dates[0].toISOString().split('T')[0] : '';
  }

  limpiarFiltros() {
    this.filtroTexto = '';
    this.fechaDesde = '';
    this.fechaHasta = '';
    this.fpDesde?.clear();
    this.fpHasta?.clear();
  }

  limpiarTextoBusqueda(): void {
    this.filtroTexto = '';
  }

  fpDesde?: flatpickr.Instance;
  fpHasta?: flatpickr.Instance;
  onMouseEnter() {
    this.isHovered = true;
  }
  onMouseLeave() {
    this.isHovered = false;
  }
}
