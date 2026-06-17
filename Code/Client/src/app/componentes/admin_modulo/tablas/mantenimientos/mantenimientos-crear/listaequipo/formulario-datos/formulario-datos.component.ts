import { Component, Input, signal, WritableSignal } from '@angular/core';
import { MantenimientosServiceEquipos } from '../../../../../../../services/mantenimientoEquipos/mantenimientosEquipos.service';
import { FormsModule } from '@angular/forms';
import { Equipos } from '../../../../../../../models/admin/Equipos';
import { CustomSelectComponent, OpcionSelect } from '../../../../../../compartidos/custom-select/custom-select.component';
@Component({
  selector: 'app-formulario-datos',
  imports: [FormsModule, CustomSelectComponent],
  templateUrl: './formulario-datos.component.html',
  styleUrl: './formulario-datos.component.css'
})
export class FormularioDatosComponent {
  @Input() agregarEquipoSeleccionado: WritableSignal<boolean> = signal(false);
  @Input() equipo : Equipos = {} as Equipos;
  @Input() agregarequipo: WritableSignal<boolean> = signal(true);
  constructor(private readonly mantenimiento : MantenimientosServiceEquipos){}
  tipomantenimiento: string = '';
  descripcion: string = '';
  submitted: boolean = false;
  tipoOpciones: OpcionSelect[] = [
    { value: 'preventivo', label: 'Preventivo' },
    { value: 'correctivo', label: 'Correctivo' },
  ];

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
