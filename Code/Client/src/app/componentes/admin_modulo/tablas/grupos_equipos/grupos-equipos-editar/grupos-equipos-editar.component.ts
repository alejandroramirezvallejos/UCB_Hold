import { Component, EventEmitter, Input, Output, signal, WritableSignal, OnChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GrupoEquipo } from '../../../../../models/grupo_equipo';
import { GrupoequipoService } from '../../../../../services/APIS/GrupoEquipo/grupoequipo.service';

@Component({
  selector: 'app-grupos-equipos-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './grupos-equipos-editar.component.html',
  styleUrl: './grupos-equipos-editar.component.css'
})
export class GruposEquiposEditarComponent implements OnChanges {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() categorias: string[] = [];
  @Input() grupoequipo: GrupoEquipo = {
    id: 0,
    nombre: '',
    modelo: '',
    marca: '',
    nombreCategoria: '',
    descripcion: '',
    url_data_sheet: '',
    link: ''
  };
  grupoEquipo: GrupoEquipo = { ...this.grupoequipo };

  constructor(private grupoEquipoapi: GrupoequipoService) { }

  ngOnChanges() {
    this.grupoEquipo = { ...this.grupoequipo };
  }

  confirmar() {
    this.grupoEquipoapi.editarGrupoEquipo(this.grupoEquipo).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
