import { Component, EventEmitter, Input, input, Output, output, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-aviso',
  imports: [],
  templateUrl: './aviso.component.html',
  styleUrl: './aviso.component.css'
})
export class Aviso {

  @Input() cerrar : WritableSignal<boolean> = signal(true);
  @Input() mensaje : string="" ; 
  @Output() aceptar : EventEmitter<void> = new EventEmitter<void>();

  click(){
    this.cerrar.set(!this.cerrar());
  }

  onAceptar(){
    this.aceptar.emit();
  }
  

}
