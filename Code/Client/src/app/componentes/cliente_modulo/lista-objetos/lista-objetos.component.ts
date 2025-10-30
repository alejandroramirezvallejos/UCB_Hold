import { Component, Input, OnChanges, signal, SimpleChanges, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { Router } from '@angular/router';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';

@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule , MostrarerrorComponent],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent implements OnChanges {
  @Input() categorias: string[] = [];
  @Input() producto: string = '';

  private todosLosProductos: GrupoEquipo[] = [];
  productosFiltrados: GrupoEquipo[] = [];
  productosPaginados: GrupoEquipo[][] = [];
  cantidadObjetos: number = 20;
  paginaActual: number = 0;
  totalPaginas: number = 0;

  error : WritableSignal<boolean> = signal(false);
  mensajeerror : string = "";

  constructor(private servicio: GrupoequipoService) {}

  ngOnInit(): void {
    this.cargarProductos();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['categorias'] || changes['producto']) && this.todosLosProductos.length > 0) {
      this.filtrarProductos();
    }
  }

  private cargarProductos(): void {
    this.servicio.getGrupoEquipo('', '').subscribe({
      next: (data) => {
        this.todosLosProductos = data;
        this.filtrarProductos();
      },
      error: (error) =>{ 
        this.mensajeerror = "Error al cargar los productos, intente mas tarde";
        console.error('Error en componente:', error)
        this.error.set(true);
      }
    });
  }

  private filtrarProductos(): void {
    let productos = [...this.todosLosProductos];

    // Filtrar por categorías
    if (this.categorias.length > 0) {
      productos = productos.filter(p =>
        this.categorias.includes(p.nombreCategoria || '')
      );
    }

    // Filtrar por término de búsqueda
    if (this.producto) {
      const busqueda = this.producto.toLowerCase();
      productos = productos.filter(p =>
        (p.nombre?.toLowerCase().includes(busqueda) ||
          p.modelo?.toLowerCase().includes(busqueda) ||
         p.marca?.toLowerCase().includes(busqueda))
      );
    }

    productos = productos.filter(p => (p.Cantidad ?? 0) > 0);



    this.productosFiltrados = productos;
    this.productosPaginados = this.paginar(productos);
    this.totalPaginas = this.productosPaginados.length;

    // Asegurarse de que la página actual sea válida
    if (this.paginaActual >= this.totalPaginas) {
      this.paginaActual = Math.max(0, this.totalPaginas - 1);
    }
  }

  paginar(productos: GrupoEquipo[]): GrupoEquipo[][] {
    const resultado: GrupoEquipo[][] = [];
    for (let i = 0; i < productos.length; i += this.cantidadObjetos) {
      resultado.push(productos.slice(i, i + this.cantidadObjetos));
    }
    return resultado;
  }

  obtenerRangoPaginas(): number[] {
    const maxBotones = 5;
    
    // Si hay menos páginas que el máximo, mostrar todas
    if (this.totalPaginas <= maxBotones) {
      return Array.from({ length: this.totalPaginas }, (_, i) => i);
    }

    // Siempre mostrar exactamente 5 botones
    const rangoMedio = Math.floor(maxBotones / 2);
    let inicio = this.paginaActual - rangoMedio;
    
    // Ajustar si está muy cerca del inicio
    if (inicio < 0) {
      inicio = 0;
    }
    
    // Ajustar si está muy cerca del final
    if (inicio + maxBotones > this.totalPaginas) {
      inicio = this.totalPaginas - maxBotones;
    }

    // Siempre retornar exactamente 5 botones
    return Array.from({ length: maxBotones }, (_, i) => inicio + i);
  }

  actualizarPagina(pagina: number): void {
    if (pagina >= 0 && pagina < this.totalPaginas) {
      this.paginaActual = pagina;
    }
  }

  get productosActuales(): GrupoEquipo[] {
    return this.productosPaginados[this.paginaActual] || [];
  }

  get hayPaginasAnteriores(): boolean {
    return this.paginaActual > 0;
  }

  get hayPaginasSiguientes(): boolean {
    return this.paginaActual < this.totalPaginas - 1;
  }
}
