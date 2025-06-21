import { Component, Input, signal, WritableSignal } from '@angular/core';
import { Mantenimientos } from '../../../../../../models/admin/Mantenimientos';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-detalles-mantenimiento',
  imports: [CommonModule],
  templateUrl: './detalles-mantenimiento.component.html',
  styleUrl: './detalles-mantenimiento.component.css'
})
export class DetallesMantenimientoComponent {
 @Input() mantenimientos :  Mantenimientos[] = [];
 @Input() mostrardetalles: WritableSignal<boolean> = signal(true);


  cerrarDetalles() {
    this.mostrardetalles.set(false);
  }

}
