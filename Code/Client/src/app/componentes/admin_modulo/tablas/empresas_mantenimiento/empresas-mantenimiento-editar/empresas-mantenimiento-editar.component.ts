import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';

@Component({
  selector: 'app-empresas-mantenimiento-editar',
  imports: [FormsModule],
  templateUrl: './empresas-mantenimiento-editar.component.html',
  styleUrl: './empresas-mantenimiento-editar.component.css'
})
export class EmpresasMantenimientoEditarComponent {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() empresaMantenimiento : EmpresaMantenimiento ={
    Id: 0,
    NombreEmpresa: '',
    NombreResponsable: '',
    ApellidoResponsable: '',
    Telefono: '',
    Nit: '',
    Direccion: ''
  };

  constructor(private empresaMantenimientoapi: EmpresamantenimientoService) {}; 

  


  confirmar (){
 
    this.empresaMantenimientoapi.actualizarEmpresaMantenimiento(this.empresaMantenimiento).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
        this.cerrar();
      }
    );
  }

  cerrar(){
    this.botoneditar.set(false);
  }

}
