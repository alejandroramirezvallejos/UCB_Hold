import { CommonModule } from '@angular/common';
import { Component,EventEmitter ,input, Output } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ObjetoComponent } from '../clic_objeto/objeto.component';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css'
})
export class PantallaMainComponent {
  @Output() itemSelected = new EventEmitter<string>();


  selectedItem: any;

  items = [
    { name: 'Portátil', image: 'assets/laptop.jpg' },
    { name: 'Proyector', image: 'assets/proyector.jpg' },
    { name: 'Cámara', image: 'assets/camara.jpg' },
    { name: 'Micrófono', image: 'assets/microfono.jpg' },
    { name: 'Tableta Gráfica', image: 'assets/tableta.jpg' },
    { name: 'Trípode', image: 'assets/tripode.jpg' }
  ];


  selectItem(item: any) {
    this.itemSelected.emit(item.name);
  }

  handleImageError(item: any) {
    item.image = null; // Elimina la imagen fallida
  }
}
