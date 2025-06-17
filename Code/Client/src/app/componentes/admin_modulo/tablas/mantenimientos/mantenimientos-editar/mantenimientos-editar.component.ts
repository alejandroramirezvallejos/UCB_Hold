import { Component, EventEmitter, Input, Output, signal, WritableSignal, OnChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Mantenimientos } from '../../../../../models/admin/Mantenimientos';
import { MantenimientoService } from '../../../../../services/APIS/Mantenimiento/mantenimiento.service';

@Component({
  selector: 'app-mantenimientos-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './mantenimientos-editar.component.html',
  styleUrl: './mantenimientos-editar.component.css'
})
export class MantenimientosEditarComponent implements OnChanges {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() mantenimientoOriginal: Mantenimientos = {
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

  mantenimiento: Mantenimientos = { ...this.mantenimientoOriginal };

  constructor(private mantenimientoapi: MantenimientoService) { }

  ngOnChanges() {
    this.mantenimiento = { ...this.mantenimientoOriginal };
  }

  confirmar() {
    // Convertir el objeto para que coincida con lo que espera el servicio
    const mantenimientoParaEnvio = {
      Id: this.mantenimiento.Id,
      FechaMantenimiento: this.mantenimiento.FechaMantenimiento,
      FechaFinalDeMantenimiento: this.mantenimiento.FechaFinalDeMantenimiento,
      NombreEmpresaMantenimiento: this.mantenimiento.NombreEmpresaMantenimiento,
      Costo: this.mantenimiento.Costo,
      DescripcionMantenimiento: this.mantenimiento.Descripcion,
      CodigoIMT: this.mantenimiento.CodigoImtEquipo,
      TipoMantenimiento: this.mantenimiento.TipoMantenimiento,
      DescripcionEquipo: this.mantenimiento.DescripcionEquipo
    };

    this.mantenimientoapi.editarMantenimiento(mantenimientoParaEnvio).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al editar mantenimiento: ' + error.message);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
