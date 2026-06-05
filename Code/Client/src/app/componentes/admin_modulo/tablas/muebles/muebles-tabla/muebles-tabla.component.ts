import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MueblesCrearComponent } from '../muebles-crear/muebles-crear.component';
import { MueblesEditarComponent } from '../muebles-editar/muebles-editar.component';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { GaveterosInlineComponent } from "../../../inline/gaveteros-inline.component";
import { AuditPanelComponent } from "../../../audit-panel/audit-panel.component";
import { StickyScrollDirective } from '../../../../../directives/sticky-scroll.directive';
@Component({
  selector: 'app-muebles-tabla',
  standalone: true,
  imports: [StickyScrollDirective, CommonModule, FormsModule, ReactiveFormsModule, MueblesCrearComponent, MueblesEditarComponent,AvisoEliminarComponent, MostrarerrorComponent, AvisoExitoComponent , BuscadorComponent, GaveterosInlineComponent, AuditPanelComponent],
  templateUrl: './muebles-tabla.component.html',
  styleUrl: './muebles-tabla.component.css'
})
export class MueblesTablaComponent extends Tabla implements OnInit {
  expandedRowId: number | null = null;
  auditRefresh = 0;
  activeTab: 'tabla' | 'auditoria' = 'tabla';

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }
  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  muebles: Muebles[] = [];
  mueblesFiltrados: Muebles[] = [];
  muebleSeleccionado: Muebles = new Muebles() ;
  override columnas: string[] = ['Nombre','Tipo','Ubicación','Costo','Gaveteros','Dimensiones'];
  constructor(private muebleapi: MuebleService) {
    super();
  }
  ngOnInit() {
    this.cargarMuebles();
  }
  limpiarMuebleSeleccionado() {
    this.muebleSeleccionado = new Muebles();
  }
  crearmueble() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarMuebles() {
    this.muebleapi.obtenerMuebles().subscribe({
      next: (data: Muebles[]) => {
        this.muebles = data;
        this.mueblesFiltrados = [...this.muebles];
        this.aplicarFiltros();
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar los muebles. Intente más tarde.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  buscar() {
    this.aplicarFiltros();
  }
  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
    const busquedaNormalizada = this.normalizeText(event[0]);
    this.mueblesFiltrados = this.muebles.filter(mueble => {
        const dimensiones = `${mueble.Longitud || ''}x${mueble.Profundidad || ''}x${mueble.Altura || ''}`;
      switch (event[1]) {
        case 'Nombre':
          return this.normalizeText(mueble.Nombre || '').includes(busquedaNormalizada);
        case 'Tipo':
          return this.normalizeText(mueble.Tipo || '').includes(busquedaNormalizada);
        case 'Ubicación':
          return this.normalizeText(mueble.Ubicacion || '').includes(busquedaNormalizada);
        case 'Costo':
          return this.normalizeText(String(mueble.Costo || '')).includes(busquedaNormalizada);
        case 'Gaveteros':
          return this.normalizeText(String(mueble.NumeroGaveteros || '')).includes(busquedaNormalizada);
        case 'Dimensiones':
          return this.normalizeText(dimensiones).includes(busquedaNormalizada);
        default:  // 'Todas las columnas'
          return this.normalizeText(mueble.Nombre || '').includes(busquedaNormalizada) ||
                 this.normalizeText(mueble.Tipo || '').includes(busquedaNormalizada) ||
                 this.normalizeText(mueble.Ubicacion || '').includes(busquedaNormalizada) ||
                 this.normalizeText(String(mueble.Costo || '')).includes(busquedaNormalizada) ||
                 this.normalizeText(String(mueble.NumeroGaveteros || '')).includes(busquedaNormalizada) ||
                 this.normalizeText(dimensiones).includes(busquedaNormalizada);
      }
    });
    } else {
      this.mueblesFiltrados = [...this.muebles];
    }
  }
  limpiarBusqueda() {
    this.aplicarFiltros();
  }
  editarMueble(mueble: Muebles) {
    this.botoncrear.set(false);
    this.muebleSeleccionado = { ...mueble };
    this.botoneditar.set(true);
  }
  eliminarMueble(mueble: Muebles) {
    this.muebleSeleccionado = mueble;
    this.alertaeliminar = true;
  }
  confirmarEliminacion() {
    this.muebleapi.eliminarMueble(this.muebleSeleccionado.Id).subscribe({
      next: (response) => {
        this.mensajeexito = 'Mueble eliminado exitosamente.';
        this.exito.set(true);
        this.auditRefresh++;
        this.cargarMuebles();
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al eliminar el mueble. Intente más tarde.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
    this.limpiarMuebleSeleccionado();
    this.alertaeliminar = false;
  }
  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarMuebleSeleccionado();
  }
}
