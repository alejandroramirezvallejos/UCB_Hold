import { CommonModule, DOCUMENT, NgOptimizedImage } from '@angular/common';
import {
  Component,
  Inject,
  Renderer2,
  signal,
  WritableSignal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  AvisoDisponibilidadService,
  DisponibilidadService,
} from '@entities/availability';
import { Carrito } from '@entities/cart';
import { GrupoEquipo, GrupoequipoService } from '@entities/equipment-group';
import { CalendarioComponent } from '@features/availability-selector';
import { CarritoService } from '@features/cart';
import { ImageCacheService } from '@shared/lib/image/image-cache.service';
import {
  EquipmentImagePlaceholderComponent,
  MostrarerrorComponent,
} from '@shared/ui';

const MINIMUM_CART_QUANTITY = 1;
const FALLBACK_MAXIMUM_QUANTITY = 99;

@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [
    CommonModule,
    MostrarerrorComponent,
    EquipmentImagePlaceholderComponent,
    CalendarioComponent,
    FormsModule,
    NgOptimizedImage,
  ],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css',
})
export class ObjetoComponent {
  readonly minimumCartQuantity = MINIMUM_CART_QUANTITY;
  id: string = '';
  producto: GrupoEquipo = new GrupoEquipo();
  cantidadDisponible: number = 0;
  totalOperativo: number = 0;
  cargando: boolean = true;
  addedToCart = false;
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = '';
  deshabilitarBoton = false;
  sinUnidadesOperativas: boolean = false;

  cantidad: number = 1;

  showCalendarioModal = false;
  carritoCalendario: Carrito = {};
  fechaInicioCalendario: WritableSignal<Date | null> = signal(null);
  fechaFinCalendario: WritableSignal<Date | null> = signal(null);

  showAvisoModal = false;
  avisoFecha = '';
  avisoRegistrado = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly servicio: GrupoequipoService,
    private readonly carrito: CarritoService,
    private readonly disponibilidadService: DisponibilidadService,
    private readonly avisoDisponibilidad: AvisoDisponibilidadService,
    private readonly imageCache: ImageCacheService,
    private readonly renderer: Renderer2,
    @Inject(DOCUMENT) private readonly document: Document,
  ) {}

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (!routeId) return;
    this.id = routeId;
    this.servicio.getproducto(routeId).subscribe({
      next: (data) => {
        this.producto = data;

        if (this.producto?.id) {
          this.carritoCalendario = {
            [this.producto.id]: {
              nombre: this.producto.nombre ?? '',
              modelo: this.producto.modelo ?? '',
              marca: this.producto.marca ?? '',
              cantidad: 1,
              fecha_inicio: null,
              fecha_final: null,
              imagen: this.producto.link ?? '',
              precio: this.producto.CostoPromedio ?? 0,
              cantidadMax: this.producto.Cantidad ?? MINIMUM_CART_QUANTITY,
            },
          };
          this.obtenerDisponibilidad();
        } else {
          this.cargando = false;
        }
      },
      error: () => {
        this.deshabilitarBoton = true;
        this.mensajeerror =
          'No se pudo cargar la información del equipo. Por favor, intenta más tarde.';
        this.cargando = false;
        this.error.set(true);
      },
    });
  }

  obtenerDisponibilidad(): void {
    this.disponibilidadService
      .obtenerDisponibilidad(new Date(), new Date(), [this.producto.id])
      .subscribe({
        next: (data) => {
          if (data?.length > 0) {
            this.cantidadDisponible = data[0].CantidadDisponible;
            this.totalOperativo = data[0].TotalOperativo ?? 0;
          }

          if (this.totalOperativo === 0) {
            this.sinUnidadesOperativas = true;
            this.deshabilitarBoton = true;
          }

          this.cargando = false;
        },
        error: () => {
          this.mensajeerror =
            'No se pudo obtener la disponibilidad del equipo. Por favor, intenta más tarde.';
          this.error.set(true);
          this.cargando = false;
        },
      });
  }

  incrementar(): void {
    if (
      this.cantidad <
      Math.min(
        this.cantidadDisponible,
        this.producto.Cantidad ?? FALLBACK_MAXIMUM_QUANTITY,
      )
    ) {
      this.cantidad++;
    }
  }

  decrementar(): void {
    if (this.cantidad > MINIMUM_CART_QUANTITY) this.cantidad--;
  }

  abrirCalendarioModal(): void {
    this.showCalendarioModal = true;
    this.renderer.setStyle(this.document.body, 'overflow', 'hidden');
  }

  cerrarCalendarioModal(): void {
    this.showCalendarioModal = false;
    this.renderer.removeStyle(this.document.body, 'overflow');
  }

  onAvisarDisponibilidad(fecha: string): void {
    this.avisoFecha = fecha;
    this.avisoRegistrado = false;
    this.showAvisoModal = true;
  }

  confirmarAviso(): void {
    if (!this.producto.id) return;

    this.avisoDisponibilidad
      .registrar(this.producto.id, this.avisoFecha)
      .subscribe({
        next: () => (this.avisoRegistrado = true),
        error: () => {
          this.mensajeerror = 'No se pudo registrar el aviso.';
          this.error.set(true);
          this.showAvisoModal = false;
        },
      });
  }

  cerrarAvisoModal(): void {
    this.showAvisoModal = false;
    this.avisoRegistrado = false;
  }

  ocultarImagenProducto(): void {
    if (this.producto.link) this.imageCache.markFailed(this.producto.link);
    this.producto.link = null;
  }

  obtenerImagenProducto(): string | null {
    const imageUrl = this.producto.link?.trim();

    if (!imageUrl) return null;
    if (!this.imageCache.canDisplay(imageUrl)) return null;
    if (this.imageCache.hasFailed(imageUrl)) return null;

    return imageUrl;
  }

  detenerPropagacion(event: Event): void {
    event.stopPropagation();
  }

  alternarProductoCarrito(): void {
    if (this.addedToCart) {
      this.carrito.quitarProducto(this.producto.id);
      this.addedToCart = false;

      return;
    }

    for (let i = 0; i < this.cantidad; i++) {
      this.carrito.agregarProducto(
        this.producto.id,
        this.producto.nombre,
        this.producto.link ?? '',
        this.producto.marca ?? '',
        this.producto.modelo ?? '',
        this.producto.CostoPromedio ?? 0,
        this.producto.Cantidad ?? MINIMUM_CART_QUANTITY,
      );
    }
    this.addedToCart = true;
  }
}
