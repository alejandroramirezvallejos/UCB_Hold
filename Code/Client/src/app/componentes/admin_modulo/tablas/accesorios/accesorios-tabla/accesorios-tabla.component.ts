import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Accesorio } from '../../../../../models/admin/Accesorio';
import { AccesoriosCrearComponent } from '../accesorios-crear/accesorios-crear.component';
import { AccesoriosEditarComponent } from '../accesorios-editar/accesorios-editar.component';
import { AccesoriosService } from '../../../../../services/APIS/Accesorio/accesorios.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';
import { extractErrorMessage } from '../../../../../utils/error-handler';
import { AuditPanelComponent } from "../../../audit-panel/audit-panel.component";
@Component({
  selector: 'app-accesorios-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule , AccesoriosCrearComponent , AccesoriosEditarComponent , AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent , Aviso , AvisoExitoComponent, BuscadorComponent, AuditPanelComponent],
  templateUrl: './accesorios-tabla.component.html',
  styleUrls: ['./accesorios-tabla.component.css']
})
export class AccesoriosTablaComponent  extends Tabla{
  expandedRowId: number | null = null;
  auditRefresh = 0;
  activeTab: 'tabla' | 'auditoria' = 'tabla';

  toggleExpand(id: number) {
    this.expandedRowId = this.expandedRowId === id ? null : id;
  }

  botoncrear : WritableSignal<boolean> = signal(false);
  botoneditar : WritableSignal<boolean> = signal(false);
  alertaeliminar : boolean = false;
  accesorios : Accesorio[] = [];
  accesorioscopia: Accesorio[] = [];
  accesorioSeleccionado:  Accesorio= new Accesorio();
  override columnas: string[] = ['Nombre','Modelo','Tipo','Código IMT del Equipo','Precio'];

  constructor(private accesoriosapi : AccesoriosService){
    super();
  };

  ngOnInit(){
    this.cargarAccesorios();
  }

  limpiarAccesorioSeleccionado() {
    this.accesorioSeleccionado = new Accesorio();
  }

  crearaccesorio() {
    this.botoneditar.set(false);
    this.botoncrear.set(true);
  }

  cargarAccesorios() {
    this.accesoriosapi.obtenerAccesorios().subscribe({
      next: (data: Accesorio[]) => {
        this.accesorios = data;
        this.accesorioscopia = [...this.accesorios];
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al cargar los accesorios. Por favor, intente más tarde.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }


  aplicarFiltros(event?: [string, string]) {
    if (event && event[0].trim() !== '') {
      const busquedaNormalizada = this.normalizeText(event[0]);
      this.accesorios = this.accesorioscopia.filter(accesorio => {
        switch (event[1]) {
          case 'Nombre':
            return this.normalizeText(accesorio.nombre || '').includes(busquedaNormalizada);
          case 'Modelo':
            return this.normalizeText(accesorio.modelo || '').includes(busquedaNormalizada);
          case 'Tipo':
            return this.normalizeText(accesorio.tipo || '').includes(busquedaNormalizada);
          case 'Código IMT del Equipo':
            return this.normalizeText(String(accesorio.codigo_imt || '')).includes(busquedaNormalizada);
          case 'Precio':
            return this.normalizeText(String(accesorio.precio || '')).includes(busquedaNormalizada);
          default:  // 'Todas las columnas'
            return this.normalizeText(accesorio.nombre || '').includes(busquedaNormalizada) ||
                  this.normalizeText(accesorio.modelo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(accesorio.tipo || '').includes(busquedaNormalizada) ||
                  this.normalizeText(String(accesorio.codigo_imt || '')).includes(busquedaNormalizada) ||
                  this.normalizeText(accesorio.nombreEquipoAsociado || '').includes(busquedaNormalizada) ||
                  this.normalizeText(String(accesorio.precio || '')).includes(busquedaNormalizada);
        }
      });
    } else {
      this.accesorios = [...this.accesorioscopia];
    }
  }


limpiarBusqueda(){
  this.accesorios = [...this.accesorioscopia];
}

editarAccesorio(accesorio : Accesorio) {
  this.botoncrear.set(false);
  this.accesorioSeleccionado = { ...accesorio }; // Crear una copia del objeto
  this.botoneditar.set(true);
}

eliminarAccesorio(accesorio : Accesorio) {
  this.accesorioSeleccionado = accesorio;
  this.alertaeliminar = true;
}

confirmarEliminacion() {
  this.accesoriosapi.eliminarAccesorio(this.accesorioSeleccionado.Id).subscribe({
    next: (response) => {
      this.cargarAccesorios();
      this.mensajeexito = 'Accesorio eliminado exitosamente.';
      this.exito.set(true);
        this.auditRefresh++;
    },
    error: (error) => {
        const errorMsg = extractErrorMessage(error, "Error al eliminar el accesorio. Por favor, intente más tarde.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
  });
  this.limpiarAccesorioSeleccionado();
  this.alertaeliminar = false;
}


cancelarEliminacion(){
  this.alertaeliminar = false;
  this.limpiarAccesorioSeleccionado();
}
}
