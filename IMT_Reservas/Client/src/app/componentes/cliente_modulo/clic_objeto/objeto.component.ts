import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductoService } from '../../../services/producto/producto.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { CarritoService } from '../../../services/carrito/carrito.service';

@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css',
})
export class ObjetoComponent {
  @Input() id: string = '';

  producto: GrupoEquipo = {
    id: 0,
    nombre: '',
    descripcion: '',
    modelo: '',
    marca: '',
    url_data_sheet: '',
    link: '',
  };

  addedToCart = false;

  constructor(
    private route: ActivatedRoute,
    private servicio: ProductoService,
    private carrito: CarritoService,
    private router: Router
  ) {}
  // objeto.component.ts
  //WARNING  :  no tocar por que no se como solucionarlo si sale error

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (!routeId) {
      console.error('ID no proporcionado en la URL');
      return;
    }
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => (this.producto = data), // Ya no accedes con [0]
      error: (error) => console.error('Error en componente:', error),
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

  userRating: number = 0;

  ratings: { [key: number]: number } = {
    5: 3,
    4: 1,
    3: 1,
    2: 0,
    1: 0,
  };

  rateProduct(rating: number) {
    this.userRating = rating;
    this.ratings[rating] += 1;
  }
  getRatingPercentage(rating: number): number {
    const total = Object.values(this.ratings).reduce((a, b) => a + b, 0);
    if (total === 0) return 0;
    return Math.round((this.ratings[rating] / total) * 100);
  }
}
