import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Muebles } from '../../../../../models/admin/Muebles';
import { MuebleService } from '../../../../../services/APIS/Mueble/mueble.service';

@Component({
  selector: 'app-muebles-crear',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './muebles-crear.component.html',
  styleUrl: './muebles-crear.component.css'
})
export class MueblesCrearComponent {

  @Input() botoncrear: WritableSignal<boolean> = signal(true);
  @Output() Actualizar = new EventEmitter<void>();

  mueble: Muebles = new Muebles();

  constructor(private muebleapi: MuebleService) { }

  registrar() {
    this.muebleapi.crearMueble(this.mueble).subscribe({
      next: (response) => {
        this.Actualizar.emit();
        this.cerrar();
      },
      error: (error) => {
        alert(error.error.error + ': ' + error.error.mensaje);
      }
    });
  }

  cerrar() {
    this.botoncrear.set(false);
  }
}
