import { Component, Input, signal, WritableSignal } from '@angular/core';
import { MantenimientosServiceEquipos } from '../../../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../../../models/admin/Equipos';

@Component({
  selector: 'app-formulario-datos',
  imports: [FormsModule],
  templateUrl: './formulario-datos.component.html',
  styleUrl: './formulario-datos.component.css'
})
export class FormularioDatosComponent {

  @Input() agregarEquipoSeleccionado: WritableSignal<boolean> = signal(false);
  @Input() equipo : Equipos = {} as Equipos;  

  constructor(private mantenimiento : MantenimientosServiceEquipos){}

  tipomantenimiento: string = '';
  descripcion: string = '';

  agregarEquipo() {
    this.mantenimiento.agregarEquipoMantenimiento(this.equipo.CodigoImt!, this.tipomantenimiento, this.descripcion , this.equipo.NombreGrupoEquipo!);
    this.cerrar();
  }

  cerrar() {
    this.agregarEquipoSeleccionado.set(false);
  }
}
