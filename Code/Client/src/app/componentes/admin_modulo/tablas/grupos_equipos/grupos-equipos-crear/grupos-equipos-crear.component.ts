import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';

@Component({
  selector: 'app-grupos-equipos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './grupos-equipos-crear.component.html',
  styleUrl: './grupos-equipos-crear.component.css'
})
export class GruposEquiposCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  grupoEquipo: GrupoEquipo = {
    id: 0,
    nombre: '',
    modelo: '',
    marca: '',
    nombreCategoria: '',
    descripcion: '',
    url_data_sheet: '',
    link: ''
  };

  constructor(private grupoEquipoapi: GrupoequipoService) { }

  registrar() {
    this.grupoEquipoapi.crearGrupoEquipo(this.grupoEquipo).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al crear grupo de equipo: ' + error);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
