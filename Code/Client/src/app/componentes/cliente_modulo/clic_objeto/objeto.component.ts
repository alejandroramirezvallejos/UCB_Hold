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

  producto: GrupoEquipo = new GrupoEquipo();

   addedToCart = false;

  constructor(private route: ActivatedRoute , private servicio : GrupoequipoService, private carrito : CarritoService) { }


  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (!routeId) {
      console.error('ID no proporcionado en la URL');
      return;
    }
    
    this.id = routeId;
    
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => this.producto = data,
      error: (error) => {
        console.error('Error completo del backend:', error.error.mensaje);
        this.producto = {
          id: 0,
          nombre: 'Error de carga',
          descripcion: 'No se pudo cargar la información del producto. Intente más tarde.',
          modelo: '',
          marca: '',
          url_data_sheet: '',
          link: ''
        };
      }
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
