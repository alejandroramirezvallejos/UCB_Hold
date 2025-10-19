import { Component, input, Input, Signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-mostrarerror',
  imports: [],
  templateUrl: './mostrarerror.component.html',
  styleUrl: './mostrarerror.component.css'
})
export class MostrarerrorComponent {


  @Input() error! : WritableSignal<boolean>  ;
  @Input() mensaje : string = "Error desconocido , intente mas tarde";

  clickx(){
    this.error.set(false);
  }

}
