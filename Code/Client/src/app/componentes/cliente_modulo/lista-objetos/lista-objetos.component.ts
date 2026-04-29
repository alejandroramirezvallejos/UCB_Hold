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
  cantidadObjetos: number = 21;
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
  private normalizeText(text: string): string {
    if (typeof text !== 'string') {
      return String(text || '').toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
    }
    return text
      .toLowerCase()
      .normalize('NFD')  // Descompone caracteres con acentos
      .replace(/[\u0300-\u036f]/g, '');  // Elimina diacríticos (acentos, tildes, etc.)
  }
  private filtrarProductos(): void {
    let productos = [...this.todosLosProductos];
    if (this.categorias.length > 0) {
      if(this.categorias.includes('sinCategoria')){
        console.log('Filtrando sin categoría');
        productos = productos.filter(p => p.nombreCategoria === null || p.nombreCategoria === ''  );
      }
      else{
        productos = productos.filter(p =>
          this.categorias.includes(p.nombreCategoria || '')
        );
      }
    }
    if (this.producto) {
      const busquedaNormalizada = this.normalizeText(this.producto);
      productos = productos.filter(p =>
        this.normalizeText(p.nombre || '').includes(busquedaNormalizada) ||
        this.normalizeText(p.modelo || '').includes(busquedaNormalizada) ||
        this.normalizeText(p.marca || '').includes(busquedaNormalizada)
      );
    }
    productos = productos.filter(p => (p.Cantidad ?? 0) > 0);
    this.productosFiltrados = productos;
    this.productosPaginados = this.paginar(productos);
    this.totalPaginas = this.productosPaginados.length;
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
    if (this.totalPaginas <= maxBotones) {
      return Array.from({ length: this.totalPaginas }, (_, i) => i);
    }
    const rangoMedio = Math.floor(maxBotones / 2);
    let inicio = this.paginaActual - rangoMedio;
    if (inicio < 0) {
      inicio = 0;
    }
    if (inicio + maxBotones > this.totalPaginas) {
      inicio = this.totalPaginas - maxBotones;
    }
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
