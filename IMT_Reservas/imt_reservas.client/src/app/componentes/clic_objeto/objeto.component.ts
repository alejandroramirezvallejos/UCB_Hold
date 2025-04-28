// objeto.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

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


  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    // Se captura el parámetro "nombre" de la URL
    const nombre = this.route.snapshot.paramMap.get('nombre');
    if (nombre) {
      this.objeto.nombre = nombre;
      // Aquí puedes cargar la descripción e imagen según tu lógica o servicio.
      this.objeto.descripcion = `Descripción detallada para el objeto ${nombre}`;
      this.objeto.imagenUrl = `assets/${nombre.toLowerCase()}.jpg`;
    }
  }


}


