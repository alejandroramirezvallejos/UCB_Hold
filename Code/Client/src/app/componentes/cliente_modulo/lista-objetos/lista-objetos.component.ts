import { Component, Input, OnChanges, OnDestroy, signal, SimpleChanges, WritableSignal, HostListener, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { Router } from '@angular/router';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { CarritoService } from '../../../services/carrito/carrito.service';
import { DisponibilidadService } from '../../../services/APIS/Disponibilidad/disponibilidad.service';
import { extractErrorMessage } from '../../../utils/error-handler';
@Component({
  selector: 'app-lista-objetos',
  standalone: true,
  imports: [RouterModule, MostrarerrorComponent, CommonModule],
  templateUrl: './lista-objetos.component.html',
  styleUrl: './lista-objetos.component.css'
})
export class ListaObjetosComponent implements OnChanges, OnDestroy, AfterViewInit {
  @ViewChild('gridContainer') gridContainer!: ElementRef;
  @Input() categorias: string[] = [];
  @Input() producto: string = '';
  private todosLosProductos: GrupoEquipo[] = [];
  productosFiltrados: GrupoEquipo[] = [];
  productosPaginados: GrupoEquipo[][] = [];
  cantidadObjetos: number = 21;
  paginaActual: number = 0;
  totalPaginas: number = 0;
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = '';

  // Cart quantity per product
  cantidades: { [id: number]: number } = {};
  addedToCart: { [id: number]: boolean } = {};
  // Total de unidades operativas por grupo (0 = sin equipos operativos)
  totalOperativo: { [id: number]: number } = {};

  constructor(
    private servicio: GrupoequipoService,
    private carrito: CarritoService,
    private disponibilidad: DisponibilidadService
  ) {}

  sinOperativos(id: number): boolean { return (this.totalOperativo[id] ?? 1) <= 0; }

  getCantidad(id: number): number { return this.cantidades[id] ?? 1; }
  incrementarCantidad(id: number, max: number, event: Event) {
    event.stopPropagation(); event.preventDefault();
    const c = this.getCantidad(id);
    if (c < (max || 99)) this.cantidades[id] = c + 1;
  }
  decrementarCantidad(id: number, event: Event) {
    event.stopPropagation(); event.preventDefault();
    const c = this.getCantidad(id);
    if (c > 1) this.cantidades[id] = c - 1;
  }
  agregarAlCarrito(item: GrupoEquipo, event: Event) {
    event.stopPropagation(); event.preventDefault();
    if (this.sinOperativos(item.id!)) return;
    if (this.addedToCart[item.id!]) {
      this.carrito.quitarproducto(item.id!);
      this.addedToCart[item.id!] = false;
      return;
    }
    const n = this.getCantidad(item.id!);
    for (let i = 0; i < n; i++) {
      this.carrito.agregarproducto(
        item.id!, item.nombre, item.link ?? '',
        item.marca ?? '', item.modelo ?? '',
        item.CostoPromedio ?? 0, item.Cantidad ?? 1
      );
    }
    this.addedToCart[item.id!] = true;
  }
  
  ngOnInit(): void {
    this.cantidadObjetos = this.servicio.cantidadObjetosGuardada || 21;
    this.paginaActual = this.servicio.paginaGuardada;
    this.cargarProductos();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.calcularCantidadObjetos();
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.calcularCantidadObjetos();
  }

  private calcularCantidadObjetos(): void {
    if (!this.gridContainer) return;

    const element = this.gridContainer.nativeElement;
    const computedStyle = window.getComputedStyle(element);
    const gridColumns = computedStyle.getPropertyValue('grid-template-columns');
    
    let columnas = 1;
    if (gridColumns && gridColumns !== 'none') {
      columnas = gridColumns.trim().split(/\s+/).length;
    }

    const filasDeseadas = 4;
    let nuevaCantidad = columnas * filasDeseadas;

    if (nuevaCantidad < 6) {
      nuevaCantidad = 6;
    }

    if (this.cantidadObjetos !== nuevaCantidad) {
      const primerItemVisible = this.paginaActual * this.cantidadObjetos;
      this.cantidadObjetos = nuevaCantidad;
      this.servicio.cantidadObjetosGuardada = nuevaCantidad;
      this.paginaActual = Math.floor(primerItemVisible / nuevaCantidad);
      this.servicio.paginaGuardada = this.paginaActual;
      if (this.todosLosProductos.length > 0) {
        this.filtrarProductos();
      }
    }
  }
  
  ngOnDestroy(): void {
    this.servicio.paginaGuardada = this.paginaActual;
    this.servicio.cantidadObjetosGuardada = this.cantidadObjetos;
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
        this.cargarOperatividad(data);
      },
      error: () => {
        this.mensajeerror = 'No se pudo cargar el catálogo de equipos. Por favor, recarga la página e inténtalo de nuevo.';
        this.error.set(true);
      }
    });
  }
  
  private cargarOperatividad(productos: GrupoEquipo[]): void {
    const ids = productos.map(p => p.id).filter((id): id is number => id != null);
    if (ids.length === 0) return;
    const hoy = new Date();
    this.disponibilidad.obtenerDisponibilidad(hoy, hoy, ids).subscribe({
      next: (data) => {
        for (const d of data) {
          this.totalOperativo[d.IdGrupoEquipo] = d.TotalOperativo ?? 0;
        }
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
      this.servicio.paginaGuardada = pagina;
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
