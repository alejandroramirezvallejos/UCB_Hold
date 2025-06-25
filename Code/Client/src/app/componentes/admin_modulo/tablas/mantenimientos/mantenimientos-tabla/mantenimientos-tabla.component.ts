import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientosCrearComponent } from '../mantenimientos-crear/mantenimientos-crear.component';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';
import { MantenimientosAgrupados } from '../../../../../models/MantenimientosAgrupados';
import { DetallesMantenimientoComponent } from './detalles-mantenimiento/detalles-mantenimiento.component';

@Component({
  selector: 'app-mantenimientos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MantenimientosCrearComponent , DetallesMantenimientoComponent],
  templateUrl: './mantenimientos-tabla.component.html',
  styleUrl: './mantenimientos-tabla.component.css'
})
export class MantenimientosTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);

  mostrardetalles : WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  mantenimientos: MantenimientosAgrupados[] = [];
  
  mantenimientoGruposeleccionado: Mantenimientos[] = [];

  mantenimientosFiltrados: MantenimientosAgrupados[] = [];

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
       this.agruparMantenimientos(data);
      },
      (error) => {
        console.error('Error al cargar los mantenimientos:', error);
      }
    );
  }

  agruparMantenimientos(datos : Mantenimientos[]) {
    this.mantenimientos = []; 

    if (datos.length === 0) return;

    let mantenimientosArray: Mantenimientos[] = [];

    for (let i = 0; i < datos.length; i++) {
      mantenimientosArray.push(datos[i]);

      if (i === datos.length - 1 || datos[i].Id !== datos[i + 1]?.Id) {
        this.mantenimientos.push(new MantenimientosAgrupados(mantenimientosArray));
        mantenimientosArray = [];
      }
    }

    this.mantenimientosFiltrados = [...this.mantenimientos];
  }


  buscar() {
    this.aplicarBusqueda();
  }

  aplicarBusqueda() {
    if (this.terminoBusqueda.trim() === '') {
      this.mantenimientosFiltrados = [...this.mantenimientos];
    } else {
      this.mantenimientosFiltrados = this.mantenimientos.filter(mantenimiento =>
        (mantenimiento.datosgrupo.NombreEmpresaMantenimiento || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.datosgrupo.TipoMantenimiento || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.datosgrupo.NombreGrupoEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.datosgrupo.Descripcion || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        (mantenimiento.datosgrupo.DescripcionEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        String(mantenimiento.datosgrupo.CodigoImtEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
        String(mantenimiento.datosgrupo.Costo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
      );
    }
  
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
    this.aplicarBusqueda();
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



  mostrarmantenimientosindividuales(mantenimientosgrupo : MantenimientosAgrupados){
    this.mantenimientoGruposeleccionado=mantenimientosgrupo.matenimientos; 
    this.mostrardetalles.set(true);
  }

  


}
