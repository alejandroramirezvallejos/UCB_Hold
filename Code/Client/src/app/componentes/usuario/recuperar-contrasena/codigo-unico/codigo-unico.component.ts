import { Component, Input, signal, WritableSignal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CambiarContrasenaComponent } from '../cambiar-contrasena/cambiar-contrasena.component';

@Component({
  selector: 'app-codigo-unico',
  imports: [FormsModule, CommonModule , CambiarContrasenaComponent],
  templateUrl: './codigo-unico.component.html',
  styleUrl: './codigo-unico.component.css'
})
export class CodigoUnicoComponent implements OnInit {
  @Input() correo: string = '';
  codigoIngresado: string = '';

  verificado : WritableSignal<boolean> = signal(false);

  ngOnInit() {
    this.enviarcodigo();
  }

  compararcodigo(codigo : string ) : boolean  {
    if (codigo === '123456') {
      this.verificado.set(true);
      return true;
    }
    this.verificado.set(false);
    return false; 
  }



  enviarcodigo() {
    alert('Código enviado a ' + this.correo);

  }
}
