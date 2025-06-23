import { Component, Input, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-vercontrato',
  imports: [CommonModule],
  templateUrl: './vercontrato.component.html',
  styleUrl: './vercontrato.component.css'
})
export class VercontratoComponent {
  @Input() vercontraro : WritableSignal<boolean> = signal(true);
   contratoContent: string = '';

  cerrar(){
    this.vercontraro.set(false);
  }
}
