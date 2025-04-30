// objeto.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductoService } from '../../services/producto/producto.service'; 
import { GrupoEquipo } from '../../models/grupo_equipo';
import { CarritoService } from '../../services/carrito/carrito.service';


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
  constructor(private route: ActivatedRoute , private servicio : ProductoService, private carrito : CarritoService, private router : Router) { }

  // objeto.component.ts
  // no tocar por que no se como solucionarlo si s
  //TODO : recordar arreglar esto para que no sea una lista sino solo un objeto 
  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');

    if (!routeId) {
      console.error('ID no proporcionado en la URL');
      return;
    }
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => this.producto = data[0],
      error: (error) => console.error('Error en componente:', error)
    });
   
  }


  addproductocarrito() {

    this.carrito.agregarproducto(this.producto.id, this.producto.nombre);

    this.router.navigate(['/home']);
    
  }


}

