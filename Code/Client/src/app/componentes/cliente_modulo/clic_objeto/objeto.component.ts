// objeto.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service'; 
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { CarritoService } from '../../../services/carrito/carrito.service';


@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css'
})
export class ObjetoComponent {
  @Input() id: string = ''

  producto: GrupoEquipo = {
    id: 0,
    nombre: '',
    descripcion: '',
    modelo: '',
    marca: '',
    url_data_sheet: '',
    link: ''
  };

   addedToCart = false;

  constructor(private route: ActivatedRoute , private servicio : GrupoequipoService, private carrito : CarritoService, private router : Router) { }

  // objeto.component.ts
  //WARNING  :  no tocar por que no se como solucionarlo si sale error

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (!routeId) {
      console.error('ID no proporcionado en la URL');
      return;
    }
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => this.producto = data, // Ya no accedes con [0]
      error: (error) => console.error('Error en componente:', error)
    });
  }


  addproductocarrito() {
    if (this.addedToCart) {
      return;
    }

    this.carrito.agregarproducto(
      this.producto.id,
      this.producto.nombre,
      this.producto.link ?? '',
      this.producto.marca ?? '',
      this.producto.modelo ?? '',
      20
    );

    this.addedToCart = true;
  }


}

