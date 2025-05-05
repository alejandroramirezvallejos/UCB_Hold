import { CommonModule } from '@angular/common';
import { Component,EventEmitter ,input, Output } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pantalla-main',
  standalone: true,
  imports: [CommonModule,ListaObjetosComponent , FormsModule],
  templateUrl: './pantalla-main.component.html',
  styleUrl: './pantalla-main.component.css'
})
export class PantallaMainComponent {
  showCategories = false;
  solicitud: string = ''; 
  categoria: string = ''; 
  items = [
    { name: 'Portátil', image: 'assets/laptop.jpg' },
    { name: 'Proyector', image: 'assets/proyector.jpg' },
    { name: 'Cámara', image: 'assets/camara.jpg' },
    { name: 'Micrófono', image: 'assets/microfono.jpg' },
    { name: 'Tableta Gráfica', image: 'assets/tableta.jpg' },
    { name: 'prueba', image: 'assets/tripode.jpg' }
  ];

  // Añade este método
  toggleCategories() {
    this.showCategories = !this.showCategories;
  }

  selectCategory(categoria : string ) {
    if (categoria == "sin categoria") {
      this.categoria = '';
    }
    else {
      this.categoria = categoria;
      
    }
    this.toggleCategories();

  }

  submitRequest() {
    if (this.solicitud.trim() || this.categoria!="") { // Asegúrate de que no esté vacío
      console.log('Solicitud enviada:', this.solicitud);
      // Aquí puedes manejar el envío de la solicitud (API, etc.)
      alert('¡Solicitud enviada con éxito!');
      this.solicitud = ''; // Limpia el campo después de enviar
    } else {
      alert('Por favor, escribe tu solicitud antes de enviarla o elije una categoria.');
    }
  }


}

