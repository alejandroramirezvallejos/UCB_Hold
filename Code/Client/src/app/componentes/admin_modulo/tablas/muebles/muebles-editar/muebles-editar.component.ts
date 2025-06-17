import { Component, EventEmitter, Input, Output, signal, WritableSignal, OnChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';

@Component({
  selector: 'app-muebles-editar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './muebles-editar.component.html',
  styleUrl: './muebles-editar.component.css'
})
export class MueblesEditarComponent implements OnChanges {
  @Input() botoneditar: WritableSignal<boolean> = signal(true);
  @Output() actualizar: EventEmitter<void> = new EventEmitter<void>();
  @Input() muebleOriginal: Muebles = {
    Id: 0,
    Nombre: '',
    NumeroGaveteros: 0,
    Ubicacion: '',
    Tipo: '',
    Costo: 0,
    Longitud: 0,
    Profundidad: 0,
    Altura: 0
  };

  mueble: Muebles = { ...this.muebleOriginal };

  constructor(private muebleapi: MuebleService) { }

  ngOnChanges() {
    this.mueble = { ...this.muebleOriginal };
  }

  confirmar() {
    this.muebleapi.actualizarMueble(this.mueble).subscribe(
      response => {
        this.actualizar.emit();
        this.cerrar();
      },
      error => {
        alert('Error al editar mueble: ' + error.message);
        this.cerrar();
      }
    );
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
