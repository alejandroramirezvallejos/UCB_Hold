import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmpresaMantenimiento } from '../../../../../models/admin/EmpresaMantenimiento';
import { EmpresamantenimientoService } from '../../../../../services/APIS/EmpresaMantenimiento/empresamantenimiento.service';

@Component({
  selector: 'app-empresas-mantenimiento-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './empresas-mantenimiento-crear.component.html',
  styleUrl: './empresas-mantenimiento-crear.component.css'
})
export class EmpresasMantenimientoCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();


  empresaMantenimiento : EmpresaMantenimiento = {
    Id: 0,
    NombreEmpresa: '',
    NombreResponsable: '',
    ApellidoResponsable: '',
    Telefono: '',
    Nit: '',
    Direccion: ''
  };


  constructor(private empresaMantenimientoapi : EmpresamantenimientoService){}; 

  // TODO : implementar
  registrar(){

    this.empresaMantenimientoapi.crearEmpresaMantenimiento(this.empresaMantenimiento).subscribe({

      next: () => {
        this.Actualizar.emit(); 
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });

  }

  cerrar(){
    this.botoncrear.set(false);
  }

}
