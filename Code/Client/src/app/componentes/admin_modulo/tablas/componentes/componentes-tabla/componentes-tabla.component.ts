import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponentesCrearComponent } from '../componentes-crear/componentes-crear.component';
import { ComponentesEditarComponent } from '../componentes-editar/componentes-editar.component';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { BuscadorComponent } from '../../../buscador/buscador.component';
import { Tabla } from '../../base/tabla';

@Component({
  selector: 'app-componentes-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ComponentesCrearComponent, ComponentesEditarComponent , AvisoEliminarComponent , MostrarerrorComponent , AvisoExitoComponent , BuscadorComponent],
  templateUrl: './componentes-tabla.component.html',
  styleUrl: './componentes-tabla.component.css'
})
export class ComponentesTablaComponent extends Tabla implements OnInit  {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  componentes: Componente[] = [];
  componentescopia: Componente[] = [];

  componenteSeleccionado: Componente = new Componente();

  override columnas: string[] = ['Nombre','Modelo','Tipo','Código IMT del Equipo','Precio Referencia'];

  
  constructor(private componenteService: ComponenteService) {
    super(); 
  }

  ngOnInit() {
    this.cargarComponentes();
  }

  limpiarComponenteSeleccionado() {
    this.componenteSeleccionado = new Componente();
  }

  crearComponente() {
    this.botoncrear.set(true);
  }

  cargarComponentes() {
    this.componenteService.obtenerComponentes().subscribe({
      next : (data: Componente[]) => {
        this.componentes = data;
        this.componentescopia = [...this.componentes];
      },
      error: (error) => {
        this.mensajeerror = "Error al cargar los componentes , Intente mas tarde";
        console.error('Error al cargar los componentes:', error);
        this.error.set(true);
      }
    });
  }

  
  aplicarFiltros(event?: [string, string]) {
   if (event && event[0].trim() !== '') {
    const busquedaNormalizada = this.normalizeText(event[0]);
    this.componentes = this.componentescopia.filter(componente => {
      switch (event[1]) {
        case 'Nombre':
          return this.normalizeText(componente.Nombre || '').includes(busquedaNormalizada);
        case 'Modelo':
          return this.normalizeText(componente.Modelo || '').includes(busquedaNormalizada);
        case 'Tipo':
          return this.normalizeText(componente.Tipo || '').includes(busquedaNormalizada);
        case 'Código IMT del Equipo':
          return this.normalizeText(String(componente.CodigoImtEquipo || '')).includes(busquedaNormalizada);
        case 'Precio Referencia':
          return this.normalizeText(String(componente.PrecioReferencia || '')).includes(busquedaNormalizada);
        default:  // 'Todas las columnas'
          return this.normalizeText(componente.Nombre || '').includes(busquedaNormalizada) ||
                 this.normalizeText(componente.Modelo || '').includes(busquedaNormalizada) ||
                 this.normalizeText(componente.Tipo || '').includes(busquedaNormalizada) ||
                 this.normalizeText(String(componente.CodigoImtEquipo || '')).includes(busquedaNormalizada) ||
                 this.normalizeText(componente.NombreEquipo || '').includes(busquedaNormalizada) ||
                 this.normalizeText(String(componente.PrecioReferencia || '')).includes(busquedaNormalizada);
      }
    });
    } else {
      // Si no hay búsqueda, restaurar la lista original
      this.componentes = [...this.componentescopia];
    }
  }

  limpiarBusqueda() {
  
    this.componentes = [...this.componentescopia];
  }

  editarComponente(componente: Componente) {
    this.botoncrear.set(false);
    this.componenteSeleccionado = { ...componente };
    this.botoneditar.set(true);
  }

  eliminarComponente(componente: Componente) {
    this.componenteSeleccionado = componente;
    this.alertaeliminar = true;
  }

  confirmarEliminacion() {
    if (this.componenteSeleccionado.Id) {
      this.componenteService.eliminarComponente(this.componenteSeleccionado.Id).subscribe({
        next: (response) => {
          this.cargarComponentes();
          this.mensajeexito = "Componente eliminado exitosamente";
          this.exito.set(true);
        },
        error: (error) => {
          this.mensajeerror = "Error al eliminar el componente, intente más tarde";
          console.error('Error al eliminar el componente: ' + error);
          this.error.set(true);
        }
      });
    }
    this.limpiarComponenteSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarComponenteSeleccionado();
  }


}
