import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GrupoequipoService } from '../../../services/APIS/GrupoEquipo/grupoequipo.service';
import { GrupoEquipo } from '../../../models/grupo_equipo';
import { CarritoService } from '../../../services/carrito/carrito.service';
import { MostrarerrorComponent } from '../../pantallas_avisos/mostrarerror/mostrarerror.component';
import { DisponibilidadService } from '../../../services/APIS/Disponibilidad/disponibilidad.service';
import { CalendarioComponent } from '../carrito/calendario/calendario.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-objeto',
  standalone: true,
  imports: [CommonModule, MostrarerrorComponent, CalendarioComponent, FormsModule],
  templateUrl: './objeto.component.html',
  styleUrl: './objeto.component.css'
})
export class ObjetoComponent {
  id: string = '';
  producto: GrupoEquipo = new GrupoEquipo();
  cantidadDisponible: number = 0;
  totalOperativo: number = 0;
  cargando: boolean = true;
  addedToCart = false;
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = '';
  desabilitarboton: boolean = false;
  sinUnidadesOperativas: boolean = false;

  // Quantity
  cantidad: number = 1;

  // Availability modal
  showCalendarioModal = false;
  closeIconHover = false;
  carritoCalendario: any = {};
  fechaInicioCalendario: WritableSignal<Date | null> = signal(null);
  fechaFinCalendario: WritableSignal<Date | null> = signal(null);

  constructor(
    private route: ActivatedRoute,
    private servicio: GrupoequipoService,
    private carrito: CarritoService,
    private SDisponibilidad: DisponibilidadService
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
              nombre: this.producto.nombre,
              modelo: this.producto.modelo ?? '',
              marca: this.producto.marca ?? '',
              cantidad: 1,
              fecha_inicio: null,
              fecha_final: null,
              imagen: this.producto.link ?? '',
              precio: this.producto.CostoPromedio ?? 0,
              cantidadMax: this.producto.Cantidad ?? 1
            }
          };
          this.obtenerDisponibilidad();
        } else {
          this.cargando = false;
        }
      },
      error: () => {
        this.desabilitarboton = true;
        this.mensajeerror = 'No se pudo cargar la información del equipo. Por favor, intenta más tarde.';
        this.cargando = false;
        this.error.set(true);
      }
    });
  }

  obtenerDisponibilidad() {
    this.SDisponibilidad.obtenerDisponibilidad(new Date(), new Date(), [this.producto.id]).subscribe({
      next: (data) => {
        if (data?.length > 0) {
          this.cantidadDisponible = data[0].CantidadDisponible;
          this.totalOperativo = data[0].TotalOperativo ?? 0;
        }
        if (this.totalOperativo === 0) {
          this.sinUnidadesOperativas = true;
          this.desabilitarboton = true;
        }
        this.cargando = false;
      },
      error: () => {
        this.mensajeerror = 'No se pudo obtener la disponibilidad del equipo. Por favor, intenta más tarde.';
        this.error.set(true);
        this.cargando = false;
      }
    });
  }

  incrementar() {
    if (this.cantidad < Math.min(this.cantidadDisponible, this.producto.Cantidad ?? 99))
      this.cantidad++;
  }

  decrementar() {
    if (this.cantidad > 1) this.cantidad--;
  }

  openCalendarioModal() { this.showCalendarioModal = true; document.body.style.overflow = 'hidden'; }
  closeCalendarioModal() { this.showCalendarioModal = false; document.body.style.overflow = ''; }

  addproductocarrito() {
    if (this.addedToCart) {
      this.carrito.quitarproducto(this.producto.id);
      this.addedToCart = false;
      return;
    }
    for (let i = 0; i < this.cantidad; i++) {
      this.carrito.agregarproducto(
        this.producto.id,
        this.producto.nombre,
        this.producto.link ?? '',
        this.producto.marca ?? '',
        this.producto.modelo ?? '',
        this.producto.CostoPromedio ?? 0,
        this.producto.Cantidad ?? 1
      );
    }
    this.addedToCart = true;
  }
}
