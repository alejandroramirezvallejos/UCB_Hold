import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponentesCrearComponent } from '../componentes-crear/componentes-crear.component';
import { ComponentesEditarComponent } from '../componentes-editar/componentes-editar.component';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';

@Component({
  selector: 'app-componentes-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ComponentesCrearComponent, ComponentesEditarComponent],
  templateUrl: './componentes-tabla.component.html',
  styleUrl: './componentes-tabla.component.css'
})
export class ComponentesTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  componentes: Componente[] = [];
  componentescopia: Componente[] = [];

  componenteSeleccionado: Componente = {
    Id: 0,
    Nombre: '',
    Modelo: '',
    Tipo: '',
    Descripcion: '',
    PrecioReferencia: 0,
    NombreEquipo: '',
    CodigoImtEquipo: 0,
    UrlDataSheet: ''
  };

  terminoBusqueda: string = '';

  // Sorting properties
  sortColumn: string = 'Nombre';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private componenteService: ComponenteService) {}

  ngOnInit() {
    this.cargarComponentes();
  }

  limpiarComponenteSeleccionado() {
    this.componenteSeleccionado = {
      Id: 0,
      Nombre: '',
      Modelo: '',
      Tipo: '',
      Descripcion: '',
      PrecioReferencia: 0,
      NombreEquipo: '',
      CodigoImtEquipo: 0,
      UrlDataSheet: ''
    };
  }

  crearComponente() {
    this.botoncrear.set(true);
  }

  cargarComponentes() {
    this.componenteService.obtenerComponentes().subscribe(
      (data: Componente[]) => {
        this.componentes = data;
        this.componentescopia = [...this.componentes];
      },
      (error) => {
        console.error('Error al cargar los componentes:', error);
      }
    );
  }

  buscar() {
    if (this.terminoBusqueda.trim() === '') {
      this.limpiarBusqueda();
      return;
    }

    this.componentes = this.componentescopia.filter(componente =>
      componente.Nombre?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      componente.Modelo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      componente.Tipo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      String(componente.CodigoImtEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      componente.NombreEquipo?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  limpiarBusqueda() {
    this.terminoBusqueda = '';
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
      this.componenteService.eliminarComponente(this.componenteSeleccionado.Id).subscribe(
        (response) => {
          this.cargarComponentes();
        },
        (error) => {
          alert('Error al eliminar el componente: ' + error);
        }
      );
    }
    this.limpiarComponenteSeleccionado();
    this.alertaeliminar = false;
  }

  cancelarEliminacion() {
    this.alertaeliminar = false;
    this.limpiarComponenteSeleccionado();
  }

  aplicarOrdenamiento() {
    this.componentes.sort((a, b) => {
      const valorA = (a as any)[this.sortColumn];
      const valorB = (b as any)[this.sortColumn];

      let compA = typeof valorA === 'string' ? valorA.toLowerCase() : valorA;
      let compB = typeof valorB === 'string' ? valorB.toLowerCase() : valorB;

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
