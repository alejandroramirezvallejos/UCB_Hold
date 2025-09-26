import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent implements OnChanges {
  @Input() categorias: string[] = [];
  @Input() producto: string = '';
  productos: GrupoEquipo[][] = [];
  todosLosProductos: GrupoEquipo[] = [];
  cantidadObjetos: number = 20;
  paginaActual: number = 0;


  constructor(private servicio: GrupoequipoService) { };

  ngOnInit(): void {
    this.servicio.getGrupoEquipo('', '').subscribe({
      next: (data) => {
        this.todosLosProductos = data;
        this.actualizarProductosFiltrados();
      },
      error: (error) => console.error('Error en componente:', error)
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['categorias'] || changes['producto']) && this.todosLosProductos.length > 0) {
      this.actualizarProductosFiltrados();
    }
  }

  private actualizarProductosFiltrados(): void {
    let productosFiltrados = this.todosLosProductos;

    if (this.categorias.length > 0) {
      productosFiltrados = this.todosLosProductos.filter(producto =>
        this.categorias.includes(producto.nombreCategoria || '')
      );
    }


    if (this.producto) {
      const busqueda = this.producto.toLowerCase();
      productosFiltrados = productosFiltrados.filter(producto =>
        producto.nombre?.toLowerCase().includes(busqueda) ||
        producto.descripcion?.toLowerCase().includes(busqueda)
      );
    }

    this.productos = this.paginar(productosFiltrados);
    this.paginaActual = 0;
  }

  paginar(productos: GrupoEquipo[]): GrupoEquipo[][] {
    const resultado: GrupoEquipo[][] = [];
    for (let i = 0; i < productos.length; i += this.cantidadObjetos) {
      resultado.push(productos.slice(i, i + this.cantidadObjetos));
    }
    return resultado;
  }

  obtenerRangoPaginas(): number[] {
    const totalPaginas = this.productos.length;
    const paginasAMostrar = 5;
    const rangoMedio = Math.floor(paginasAMostrar / 2);

    let inicio = Math.max(this.paginaActual - rangoMedio, 0);
    let fin = Math.min(inicio + paginasAMostrar - 1, totalPaginas - 1);

    if (fin - inicio + 1 < paginasAMostrar) {
      inicio = Math.max(fin - paginasAMostrar + 1, 0);
    }

    return Array.from({length: fin - inicio + 1}, (_, i) => inicio + i);
  }

  actualizarPagina(pagina: number): void {
    if (pagina >= 0 && pagina < this.productos.length) {
      this.paginaActual = pagina;
    }
  }

  mostrarPuntosSuspensivos(index: number): boolean {
    const totalPaginas = this.productos.length;
    return (index === 0 && this.paginaActual > 2) ||
           (index === totalPaginas - 1 && this.paginaActual < totalPaginas - 3);
  }

}
