import { Component, HostListener, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
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
        const errorMsg = extractErrorMessage(error, "Error al actualizar la Empresa de Mantenimiento.");
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar(){
    this.botoneditar.set(false);
  }
  @HostListener('click', ['$event'])
  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) this.cerrar();
  }

}