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
  @Input() muebleOriginal: Muebles = new Muebles();

  mueble: Muebles = { ...this.muebleOriginal };

  constructor(private muebleapi: MuebleService) { }

  ngOnChanges() {
    this.mueble = { ...this.muebleOriginal };
  }

  confirmar() {
    this.muebleapi.actualizarMueble(this.mueble).subscribe({
      next: (response) => {
        this.actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cerrar() {
    this.botoneditar.set(false);
  }
}
