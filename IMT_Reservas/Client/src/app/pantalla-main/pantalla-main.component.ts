import { CommonModule } from '@angular/common';
import { Component,EventEmitter ,input, Output } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css'
})
export class PantallaMainComponent {


  items = [
    { name: 'Portátil', image: 'assets/laptop.jpg' },
    { name: 'Proyector', image: 'assets/proyector.jpg' },
    { name: 'Cámara', image: 'assets/camara.jpg' },
    { name: 'Micrófono', image: 'assets/microfono.jpg' },
    { name: 'Tableta Gráfica', image: 'assets/tableta.jpg' },
    { name: 'Trípode', image: 'assets/tripode.jpg' }
  ];


  handleImageError(item: any) {
    item.image = null; // Elimina la imagen fallida
  }
}
