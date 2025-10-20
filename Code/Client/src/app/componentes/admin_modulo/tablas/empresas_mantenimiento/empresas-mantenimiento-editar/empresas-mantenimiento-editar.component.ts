import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';

@Component({
  selector: 'app-empresas-mantenimiento-editar',
  imports: [FormsModule, MostrarerrorComponent , Aviso ,AvisoExitoComponent],
  templateUrl: './empresas-mantenimiento-editar.component.html',
  styleUrl: './empresas-mantenimiento-editar.component.css'
})
export class EmpresasMantenimientoEditarComponent extends BaseTablaComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() empresaMantenimiento : EmpresaMantenimiento =new EmpresaMantenimiento();

  constructor(private empresaMantenimientoapi: EmpresamantenimientoService) {
    super();
  }

  validaredicion(){
    this.mensajeaviso="¿Desea guardar los cambios realizados?";
    this.aviso.set(true);

  }

  confirmar() {

    this.empresaMantenimientoapi.actualizarEmpresaMantenimiento(this.empresaMantenimiento).subscribe({
      next: () => {
        this.actualizar.emit();
        this.mensajeexito="Empresa de Mantenimiento actualizada exitosamente.";
        this.exito.set(true);
      },
      error: (error) => {
        this.mensajeerror = "Error al actualizar la Empresa de Mantenimiento.";
        console.error(error.error.error + ': ' + error.error.mensaje);
        this.error.set(true);
      }
    });
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
