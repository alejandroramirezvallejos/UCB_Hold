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
  @Input() agregarequipo: WritableSignal<boolean> = signal(true);
  constructor(private mantenimiento : MantenimientosServiceEquipos){}
  tipomantenimiento: string = '';
  descripcion: string = '';
  submitted: boolean = false;

  agregarEquipo() {
    this.submitted = true;
    if (!this.tipomantenimiento || !this.descripcion) {
      return;
    }
    this.mantenimiento.agregarEquipoMantenimiento(this.equipo.CodigoImt!, this.tipomantenimiento, this.descripcion , this.equipo.NombreGrupoEquipo!);
    this.cerrarYRegresar();
  }
  cerrar() {
    this.agregarEquipoSeleccionado.set(false);
  }
  cerrarYRegresar() {
    this.agregarEquipoSeleccionado.set(false);
    this.agregarequipo.set(false);
  }
}
