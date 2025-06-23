import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';

@Component({
  selector: 'app-mantenimientos-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './mantenimientos-crear.component.html',
  styleUrl: './mantenimientos-crear.component.css'
})
export class MantenimientosCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  mantenimiento: Mantenimientos = {
    Id: 0,
    NombreEmpresaMantenimiento: '',
    FechaMantenimiento: null,
    FechaFinalDeMantenimiento: null,
    Costo: 0,
    Descripcion: '',
    TipoMantenimiento: '',
    NombreGrupoEquipo: '',
    CodigoImtEquipo: 0,
    DescripcionEquipo: ''
  };

  constructor(private mantenimientoapi: MantenimientoService) { }

  registrar() {
    // Convertir el objeto para que coincida con lo que espera el servicio
    const mantenimientoParaEnvio = {
      FechaMantenimiento: this.mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: this.mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: this.mantenimiento.NombreEmpresaMantenimiento,
      Costo: this.mantenimiento.Costo,
      DescripcionMantenimiento: this.mantenimiento.Descripcion,
      CodigoIMT: this.mantenimiento.CodigoImtEquipo,
      TipoMantenimiento: this.mantenimiento.TipoMantenimiento,
      DescripcionEquipo: this.mantenimiento.DescripcionEquipo
    };

    this.mantenimientoapi.crearMantenimiento(mantenimientoParaEnvio).subscribe(
      response => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    );
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
