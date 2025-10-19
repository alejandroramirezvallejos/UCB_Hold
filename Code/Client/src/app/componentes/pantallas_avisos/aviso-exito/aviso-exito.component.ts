import { Component, EventEmitter, Input, Output, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-aviso-exito',
  imports: [],
  templateUrl: './aviso-exito.component.html',
  styleUrl: './aviso-exito.component.css'
})
export class AvisoExitoComponent {
  @Input() mensaje: string = "Aviso informativo desconocido";
  @Input() exito!: WritableSignal<boolean>;
  @Output() accion = new EventEmitter<void>();


  cerrar(){
      this.accion.emit();
    this.exito.set(false);

  }

}
