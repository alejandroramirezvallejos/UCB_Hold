import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { Prestamos } from '../../models/admin/Prestamos';

@Component({
  selector: 'app-vista-prestamos',
  imports: [],
  templateUrl: './vista-prestamos.component.html',
  styleUrl: './vista-prestamos.component.css'
})
export class VistaPrestamosComponent {
  @Input() prestamos: Prestamos[] = [];
  @Output() cerrar : EventEmitter<void> = new EventEmitter<void>();

  cerrarVista(){
    this.cerrar.emit();
  }
  

}
