import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';
import { BaseTablaComponent } from '../../base/base';
import { MostrarerrorComponent } from '../../../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { AvisoExitoComponent } from '../../../../pantallas_avisos/aviso-exito/aviso-exito.component';
import { Aviso } from '../../../../pantallas_avisos/aviso/aviso.component';
import { extractErrorMessage } from '../../../../../utils/error-handler';
@Component({
  selector: 'app-empresas-mantenimiento-crear',
  standalone: true,
  imports: [FormsModule,MostrarerrorComponent , Aviso,AvisoExitoComponent],
  templateUrl: './empresas-mantenimiento-crear.component.html',
  styleUrl: './empresas-mantenimiento-crear.component.css'
})
export class EmpresasMantenimientoCrearComponent  extends BaseTablaComponent{
  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();
  empresaMantenimiento : EmpresaMantenimiento = new EmpresaMantenimiento();
  constructor(private readonly empresaMantenimientoapi : EmpresamantenimientoService){
    super(); 
  }; 
  validarregistro(){
    this.mensajeaviso="Esta seguro de crear este empresa?";
    this.aviso.set(true);
  }
  registrar(){
    this.empresaMantenimientoapi.crearEmpresaMantenimiento(this.empresaMantenimiento).subscribe({
      next: () => {
        this.Actualizar.emit(); 
        this.mensajeexito = "Empresa de mantenimiento creada Satisfactoriamente";
        this.exito.set(true);
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(error, 'Error al crear la empresa, Intente mas tarde');
        this.mensajeerror = errorMsg;
        console.error(errorMsg);
        this.error.set(true);
      }
    });
  }
  cerrar(){
    this.botoncrear.set(false);
  }
}
