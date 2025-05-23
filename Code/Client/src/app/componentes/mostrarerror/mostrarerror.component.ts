import { Component, input, Input, Signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-mostrarerror',
  imports: [],
  templateUrl: './mostrarerror.component.html',
  styleUrl: './mostrarerror.component.css'
})
export class MostrarerrorComponent {
  // error es un number por si se necesitan mas estados que 0 y 1 o falso y true 
  // en este caso se usa  0 (false) 1 (true)  2(desactivado )
  //TODO : haceer un enum que contenga los 3 estados 
  @Input() error! : WritableSignal<number>  ;
  @Input() mensaje : string = "";

  clickx(){
    this.error.set(2);
  }

}
