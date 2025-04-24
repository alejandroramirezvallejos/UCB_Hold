// objeto.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css'
})
export class ObjetoComponent {
  @Input() objeto: any = {
    nombre: '',
    descripcion: '',
    imagenUrl: ''
  };
}
