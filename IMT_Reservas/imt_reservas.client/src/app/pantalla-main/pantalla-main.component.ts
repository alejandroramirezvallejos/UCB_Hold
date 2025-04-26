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
  showCategories = false;

  items = [
    { name: 'Portátil', image: 'assets/laptop.jpg' },
    { name: 'Proyector', image: 'assets/proyector.jpg' },
    { name: 'Cámara', image: 'assets/camara.jpg' },
    { name: 'Micrófono', image: 'assets/microfono.jpg' },
    { name: 'Tableta Gráfica', image: 'assets/tableta.jpg' },
    { name: 'Trípode', image: 'assets/tripode.jpg' }
  ];

  // Añade este método
  toggleCategories() {
    this.showCategories = !this.showCategories;
  }


  handleImageError(item: any) {
    item.image = null; // Elimina la imagen fallida
  }




  solicitud: string = ''; // Para guardar el contenido del textarea

  submitRequest() {
    if (this.solicitud.trim()) { // Asegúrate de que no esté vacío
      console.log('Solicitud enviada:', this.solicitud);
      // Aquí puedes manejar el envío de la solicitud (API, etc.)
      alert('¡Solicitud enviada con éxito!');
      this.solicitud = ''; // Limpia el campo después de enviar
    } else {
      alert('Por favor, escribe tu solicitud antes de enviarla.');
    }
  }


}

