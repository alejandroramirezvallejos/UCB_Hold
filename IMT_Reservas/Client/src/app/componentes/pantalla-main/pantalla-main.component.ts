import { CommonModule } from '@angular/common';
import { Component,EventEmitter ,input, Output } from '@angular/core';
import { ListaObjetosComponent } from '../lista-objetos/lista-objetos.component';
import { FormsModule } from '@angular/forms';
import { BuscadorService } from '../../services/buscador/buscador.service';

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
  enviar: boolean = false;
  items = [
    { name: 'Portátil', image: 'assets/laptop.jpg' },
    { name: 'Proyector', image: 'assets/proyector.jpg' },
    { name: 'Cámara', image: 'assets/camara.jpg' },
    { name: 'Micrófono', image: 'assets/microfono.jpg' },
    { name: 'Tableta Gráfica', image: 'assets/tableta.jpg' },
    { name: 'prueba', image: 'assets/tripode.jpg' }
  ];

  constructor(private buscador: BuscadorService) {
    this.solicitud = buscador.producto;
    this.categoria = buscador.categoria;
  }
  // Añade este método
  toggleCategories() {
    this.showCategories = !this.showCategories;
  }

  selectCategory(categoria : string ) {
    if (categoria == "sin categoria") {
      this.categoria = '';
    }
    else {
      this.categoria = categoria        ; 
      
    }
    this.toggleCategories();
    this.submitRequest();
  }

  //TODO : Optimizar esto seguro que es mala practica 
  submitRequest() {
    // Actualiza el servicio con los datos actuales
    this.buscador.producto = this.solicitud;
    this.buscador.categoria = this.categoria;
      this.enviar = false;
      setTimeout(() => {
        this.enviar = true;
      }, 0);
  }


}

