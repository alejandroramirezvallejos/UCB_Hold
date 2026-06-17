import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Equipos } from '../../../../../models/admin/Equipos';
import { EquiposCrearComponent } from '../equipos-crear/equipos-crear.component';
import { EquiposEditarComponent } from '../equipos-editar/equipos-editar.component';
import { EquipoService } from '../../../../../services/APIS/Equipo/equipo.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { HistorialEquipoInlineComponent } from "../../../inline/historial-equipo-inline.component";
import { AuditPanelComponent } from "../../../audit-panel/audit-panel.component";
import { StickyScrollDirective } from '../../../../../directives/sticky-scroll.directive';
@Component({
  selector: 'app-equipos-tabla',
  standalone: true,
  imports: [StickyScrollDirective, CommonModule, FormsModule, ReactiveFormsModule , EquiposCrearComponent , EquiposEditarComponent,AvisoEliminarComponent , MostrarerrorComponent, AvisoExitoComponent , BuscadorComponent, HistorialEquipoInlineComponent, AuditPanelComponent],
  templateUrl: './equipos-tabla.component.html',
  styleUrls: ['./equipos-tabla.component.css']
})
export class EquiposTablaComponent extends Tabla{
  expandedRowId: number | null = null;
  auditRefresh = 0;
  activeTab: 'tabla' | 'auditoria' = 'tabla';

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  obsHistorial: string | null = null;
  abrirObsHistorial(obs: string) { this.obsHistorial = obs; }
  cerrarObsHistorial() { this.obsHistorial = null; }

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);
  alertaeliminar : boolean = false;
  equipos : any[] = [];
  equiposcopia: any[] = [];
  equipoSeleccionado:  Equipos= new Equipos();
  override columnas: string[] = ['Nombre','EstadoEquipo','Ubicacion','Código IMT','Costo'];
  // Filtro por estado del equipo
  showEstados = false;
  estadoSeleccionado = '';
  estadosDisponibles = ['operativo', 'parcialmente_operativo', 'inoperativo'];
  hover = { filter: false };
  private busquedaActual?: [string, string];
  mostrarEstados() { this.showEstados = !this.showEstados; }
  seleccionarEstado(estado: string) { this.estadoSeleccionado = estado; this.showEstados = false; this.aplicarFiltros(); }
  estadoEquipoLabel(estado: string): string {
    switch (estado) {
      case 'operativo': return 'Operativo';
      case 'parcialmente_operativo': return 'Parcialmente operativo';
      case 'inoperativo': return 'Inoperativo';
      default: return estado;
    }
  }
  constructor(private readonly equiposapi : EquipoService){
    super();
  };
  ngOnInit(){
    this.cargarEquipos();
  }
  limpiarEquipoSeleccionado() {
    this.equipoSeleccionado = new Equipos();
  }
  crearequipo() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }
  cargarEquipos() {
    this.equiposapi.obtenerEquipos().subscribe({
      next: (data: any[]) => {
        this.equipos = data;
        this.equiposcopia = [...this.equipos];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar los equipos");
        this.mensajeerror = errorMsg;
        console.error('Error al cargar los equipos:', errorMsg);
        this.error.set(true);
      }
    });
  }
  aplicarFiltros(event?: [string, string]){
    if (event) this.busquedaActual = event;
    let lista = [...this.equiposcopia];
    const ev = this.busquedaActual;
    if (ev && ev[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(ev[0]);
      lista = lista.filter(equipo => {
        switch (ev[1]) {
          case 'Nombre':
            return this.normalizeText(equipo.NombreGrupoEquipo || '').includes(busquedaNormalizada);
          case 'EstadoEquipo':
            return this.normalizeText(equipo.EstadoEquipo || '').includes(busquedaNormalizada);
          case 'Ubicacion':
            return this.normalizeText(equipo.Ubicacion || '').includes(busquedaNormalizada);
          case 'Código IMT':
            return this.normalizeText(String(equipo.CodigoImt || '')).includes(busquedaNormalizada);
          case 'Costo':
            return this.normalizeText(String(equipo.CostoReferencia || '')).includes(busquedaNormalizada);
          default:
            return this.normalizeText(equipo.NombreGrupoEquipo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(equipo.EstadoEquipo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(equipo.Ubicacion || '').includes(busquedaNormalizada) ||
                  this.normalizeText(String(equipo.CodigoImt || '')).includes(busquedaNormalizada) ||
                  this.normalizeText(String(equipo.CostoReferencia || '')).includes(busquedaNormalizada);
        }
      });
    }
    if (this.estadoSeleccionado !== '') {
      lista = lista.filter(equipo => (equipo.EstadoEquipo || '') === this.estadoSeleccionado);
    }
    this.equipos = lista;
  }
limpiarBusqueda(){
  this.equipos = [...this.equiposcopia];
}
editarEquipo(equipo : any) {
  this.botoncrear.set(false);
  this.equipoSeleccionado = { ...equipo }; // Crear una copia del objeto
  this.botoneditar.set(true);
}
eliminarEquipo(equipo : any) {
  this.equipoSeleccionado = equipo;
  this.alertaeliminar = true;
}
confirmarEliminacion() {
  this.equiposapi.eliminarEquipo(this.equipoSeleccionado.Id).subscribe({
    next:(response) => {
      this.mensajeexito = "Equipo eliminado con éxito";
      this.exito.set(true);
        this.auditRefresh++;
       this.cargarEquipos();
    },
    error: (error) => {
      const errorMsg = extractErrorMessage(error, "Error al eliminar el equipo");
      this.mensajeerror = errorMsg;
      console.error('Error al eliminar el equipo:', errorMsg);
      this.error.set(true);
    }}
  );
  this.limpiarEquipoSeleccionado();
  this.alertaeliminar = false;
}
cancelarEliminacion(){
  this.alertaeliminar = false;
  this.limpiarEquipoSeleccionado();
}

  override sortTable(e: {col: string, dir: 'asc' | 'desc'}) {
    const m: Record<string, string> = {
      'Nombre': 'NombreGrupoEquipo', 'EstadoEquipo': 'EstadoEquipo',
      'Ubicacion': 'Ubicacion', 'Código IMT': 'CodigoImt', 'Costo': 'CostoReferencia'
    };
    const k = m[e.col]; if (!k) return;
    this.equipos = [...this.equipos].sort((a, b) => {
      const va = this.normalizeText(String((a as any)[k] ?? ''));
      const vb = this.normalizeText(String((b as any)[k] ?? ''));
      return e.dir === 'asc' ? va.localeCompare(vb) : vb.localeCompare(va);
    });
  }

}