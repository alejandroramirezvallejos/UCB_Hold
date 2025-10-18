import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Componente } from '../../../../../models/admin/Componente';
import { ComponentesCrearComponent } from '../componentes-crear/componentes-crear.component';
import { ComponentesEditarComponent } from '../componentes-editar/componentes-editar.component';
import { ComponenteService } from '../../../../../services/APIS/Componente/componente.service';
import { AvisoEliminarComponent } from '../../../../pantallas_avisos/aviso-eliminar/aviso-eliminar.component';

@Component({
  selector: 'app-componentes-tabla',
  standalone: true,
  imports: [CommonModule, FormsModule, ComponentesCrearComponent, ComponentesEditarComponent , AvisoEliminarComponent],
  templateUrl: './componentes-tabla.component.html',
  styleUrl: './componentes-tabla.component.css'
})
export class ComponentesTablaComponent implements OnInit {

  botoncrear: WritableSignal<boolean> = signal(false);
  botoneditar: WritableSignal<boolean> = signal(false);

  alertaeliminar: boolean = false;
  componentes: Componente[] = [];
  componentescopia: Componente[] = [];

  componenteSeleccionado: Componente = new Componente();

  terminoBusqueda: string = '';

  
  constructor(private componenteService: ComponenteService) {}

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
      (componente.Nombre|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      (componente.Modelo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      (componente.Tipo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      String(componente.CodigoImtEquipo || '').toLowerCase().includes(this.terminoBusqueda.toLowerCase()) ||
      (componente.NombreEquipo|| '').toLowerCase().includes(this.terminoBusqueda.toLowerCase())
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


}
