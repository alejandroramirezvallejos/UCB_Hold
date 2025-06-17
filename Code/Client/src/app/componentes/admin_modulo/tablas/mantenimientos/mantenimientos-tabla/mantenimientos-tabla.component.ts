import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientosCrearComponent } from '../mantenimientos-crear/mantenimientos-crear.component';
import { MantenimientosEditarComponent } from '../mantenimientos-editar/mantenimientos-editar.component';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';

@Component({
  selector: 'app-mantenimientos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MantenimientosCrearComponent, MantenimientosEditarComponent],
  templateUrl: './mantenimientos-tabla.component.html',
  styleUrl: './mantenimientos-tabla.component.css'
})
export class MantenimientosTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  mantenimientos: Mantenimientos[] = [];
  mantenimientosFiltrados: Mantenimientos[] = [];

  mantenimientoSeleccionado: Mantenimientos = {
    Id: 0,
    NombreEmpresaMantenimiento: '',
    FechaMantenimiento: null,
    FechaFinalDeMantenimiento: null,
    Costo: 0,
    Descripcion: '',
    TipoMantenimiento: '',
    NombreGrupoEquipo: '',
    CodigoImtEquipo: 0,
    DescripcionEquipo: ''
  };

  terminoBusqueda: string = '';
  sortColumn: string = 'NombreEmpresaMantenimiento';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private mantenimientoapi: MantenimientoService) { }

  ngOnInit() {
    this.cargarMantenimientos();
  }

  limpiarMantenimientoSeleccionado() {
    this.mantenimientoSeleccionado = {
      Id: 0,
      NombreEmpresaMantenimiento: '',
      FechaMantenimiento: null,
      FechaFinalDeMantenimiento: null,
      Costo: 0,
      Descripcion: '',
      TipoMantenimiento: '',
      NombreGrupoEquipo: '',
      CodigoImtEquipo: 0,
      DescripcionEquipo: ''
    };
  }

  crearmantenimiento() {
    this.botoncrear.set(true);
  }

  cargarMantenimientos() {
    this.mantenimientoapi.obtenerMantenimientos().subscribe(
      (data: Mantenimientos[]) => {
        this.mantenimientos = data;
        this.mantenimientosFiltrados = [...this.mantenimientos];
        this.aplicarBusqueda();
      },
      (error) => {
        console.error('Error al cargar los mantenimientos:', error);
      }
    );
  }

  buscar() {
    this.aplicarBusqueda();
  }

  aplicarBusqueda() {
    if (this.terminoBusqueda.trim() === '') {
      this.mantenimientosFiltrados = [...this.mantenimientos];
    } else {
      this.mantenimientosFiltrados = this.mantenimientos.filter(mantenimiento =>
        (mantenimiento.NombreEmpresaMantenimiento || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.TipoMantenimiento || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.NombreGrupoEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.Descripcion || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.DescripcionEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        String(mantenimiento.Costo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
      );
    }
    this.aplicarOrdenamiento();
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.aplicarBusqueda();
  }

  editarMantenimiento(mantenimiento: Mantenimientos) {
    this.botoncrear.set(false);
    this.mantenimientoSeleccionado = { ...mantenimiento };
    this.botoneditar.set(true);
  }

  eliminarMantenimiento(mantenimiento: Mantenimientos) {
    this.mantenimientoSeleccionado = mantenimiento;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    this.mantenimientoapi.eliminarMantenimiento(this.mantenimientoSeleccionado.Id).subscribe(
      (response) => {
        this.cargarMantenimientos();
      },
      (error) => {
        alert('Error al eliminar el mantenimiento: ' + error);
      }
    );
    this.limpiarMantenimientoSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarMantenimientoSeleccionado();
  }

  aplicarOrdenamiento() {
    this.mantenimientosFiltrados.sort((a, b) => {
      const valorA = (a as any)[this.sortColumn];
      const valorB = (b as any)[this.sortColumn];

      let compA = typeof valorA === 'string' ? valorA.toLowerCase() : valorA;
      let compB = typeof valorB === 'string' ? valorB.toLowerCase() : valorB;

      // Manejo especial para fechas
      if (this.sortColumn === 'FechaMantenimiento' || this.sortColumn === 'FechaFinalDeMantenimiento') {
        compA = valorA ? new Date(valorA).getTime() : 0;
        compB = valorB ? new Date(valorB).getTime() : 0;
      }

      if (compA < compB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      } else if (compA > compB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      } else {
        return 0;
      }
    });
  }

  ordenarPor(columna: string) {
    if (this.sortColumn === columna) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = columna;
      this.sortDirection = 'asc';
    }

    this.aplicarOrdenamiento();
  }
}
