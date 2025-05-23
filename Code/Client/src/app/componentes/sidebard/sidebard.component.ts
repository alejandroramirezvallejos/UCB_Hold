import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, input, Output, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-sidebard',
  imports: [CommonModule],
  templateUrl: './sidebard.component.html',
  styleUrl: './sidebard.component.css'
})
export class SidebardComponent {

  boton : WritableSignal<boolean> = signal(true);

  @Input() contenido : string[] = [];

  @Output() item :EventEmitter<string> = new EventEmitter<string>();

  clickboton(){
    this.boton.set(!this.boton());
  }

  clickitem(item : string){
    this.item.emit(item);
  } 

}
