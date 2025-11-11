import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientosCrearComponent } from '../mantenimientos-crear/mantenimientos-crear.component';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';
import { MantenimientosAgrupados } from '../../../../../models/MantenimientosAgrupados';
import { DetallesMantenimientoComponent } from './detalles-mantenimiento/detalles-mantenimiento.component';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';

@Component({
  selector: 'app-mantenimientos-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MantenimientosCrearComponent , DetallesMantenimientoComponent,AvisoEliminarComponent, MostrarerrorComponent, AvisoExitoComponent,BuscadorComponent],
  templateUrl: './mantenimientos-tabla.component.html',
  styleUrl: './mantenimientos-tabla.component.css'
})
export class MantenimientosTablaComponent extends Tabla implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);

  mostrardetalles : WritableSignal<boolean> = signal(false);
  alertaeliminar: boolean = false;
  mantenimientos: MantenimientosAgrupados[] = [];
  
  mantenimientoGruposeleccionado: Mantenimientos[] = [];

  mantenimientosFiltrados: MantenimientosAgrupados[] = [];

  mantenimientoSeleccionado: Mantenimientos = new Mantenimientos();

  override columnas: string[] = ['Empresa','codigosIMT','Fecha Inicio','Fecha Fin','Costo'];


  constructor(private mantenimientoapi: MantenimientoService) { 
    super();
  }

  ngOnInit() {
    this.cargarMantenimientos();
  }

  limpiarMantenimientoSeleccionado() {
    this.mantenimientoSeleccionado = new Mantenimientos();
  }

  crearmantenimiento() {
    this.botoncrear.set(true);
  }

  cargarMantenimientos() {
    this.mantenimientoapi.obtenerMantenimientos().subscribe({
      next: (datos) => {
        this.agruparMantenimientos(datos);
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los mantenimientos, intente mas tarde";
        console.error('Error al cargar los mantenimientos: ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  agruparMantenimientos(datos : Mantenimientos[]) {
    this.mantenimientos = []; 

    if (datos.length === 0) {
      this.mantenimientosFiltrados = [];
     return;
    }

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
    this.aplicarFiltros();
  }

  aplicarFiltros(event?: [string, string]) {
      if (event && event[0].trim() !== '') {
        const busquedaNormalizada = this.normalizeText(event[0]);
        this.mantenimientosFiltrados = this.mantenimientos.filter(mantenimiento => {
          switch (event[1]) {
            case 'Empresa':
              return this.normalizeText(mantenimiento.datosgrupo.NombreEmpresaMantenimiento || '').includes(busquedaNormalizada);
            case 'codigosIMT':
              return this.normalizeText(String(mantenimiento.datosgrupo.CodigoImtEquipo || '')).includes(busquedaNormalizada);
            case 'Fecha Inicio':
               const fechaFormateada = this.formatDate(mantenimiento.datosgrupo.FechaMantenimiento);
  console.log('Fecha formateada:', fechaFormateada, 'Búsqueda:', busquedaNormalizada);
  return this.normalizeText(fechaFormateada).includes(busquedaNormalizada);
              
            case 'Fecha Fin':
              return this.normalizeText(this.formatDate(mantenimiento.datosgrupo.FechaFinalDeMantenimiento)).includes(busquedaNormalizada);
            case 'Costo':
              return this.normalizeText(String(mantenimiento.datosgrupo.Costo || '')).includes(busquedaNormalizada);
            default:  // 'Todas las columnas'
              return this.normalizeText(mantenimiento.datosgrupo.NombreEmpresaMantenimiento || '').includes(busquedaNormalizada) ||
                    this.normalizeText(mantenimiento.datosgrupo.TipoMantenimiento || '').includes(busquedaNormalizada) ||
                    this.normalizeText(mantenimiento.datosgrupo.NombreGrupoEquipo || '').includes(busquedaNormalizada) ||
                    this.normalizeText(String(mantenimiento.datosgrupo.CodigoImtEquipo || '')).includes(busquedaNormalizada) ||
                    this.normalizeText(this.formatDate(mantenimiento.datosgrupo.FechaMantenimiento)).includes(busquedaNormalizada) ||
                    this.normalizeText(this.formatDate(mantenimiento.datosgrupo.FechaFinalDeMantenimiento)).includes(busquedaNormalizada) ||
                    this.normalizeText(String(mantenimiento.datosgrupo.Costo || '')).includes(busquedaNormalizada);
          }
        });
      } else {
        // Crear una copia para evitar referencias
        this.mantenimientosFiltrados = [...this.mantenimientos];
      }
  }

  limpiarBusqueda() {
    this.aplicarFiltros();
  }



  eliminarMantenimiento(mantenimiento: MantenimientosAgrupados) {
    this.mantenimientoSeleccionado = mantenimiento.matenimientos[0]; 
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
 
    this.mantenimientoapi.eliminarMantenimiento(this.mantenimientoSeleccionado.Id).subscribe({
      next: () => {
         this.limpiarMantenimientoSeleccionado();
          this.alertaeliminar = false;
          this.mensajeexito = "Mantenimiento eliminado exitosamente";
          this.exito.set(true);
          this.cargarMantenimientos();
      },
      error: (error) => {
          this.mensajeerror = "Error al eliminar el mantenimiento, intente mas tarde";
          this.error.set(true);
         console.error('Error al eliminar el mantenimiento: ' + error.error.mensaje);
          this.limpiarMantenimientoSeleccionado();
          this.alertaeliminar = false;
      }
    })
  

    
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
