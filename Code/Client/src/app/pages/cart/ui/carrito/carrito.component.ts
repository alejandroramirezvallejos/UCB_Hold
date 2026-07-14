import { CommonModule } from '@angular/common';
import { Component, computed, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Carrito } from '@entities/cart';
import { PrestamosAPIService } from '@entities/loan';
import { UsuarioService } from '@entities/user';
import { CalendarioComponent } from '@features/availability-selector';
import { CarritoService, CartDateValidationService } from '@features/cart';
import { extractErrorMessage } from '@shared/lib/error';
import {
  Aviso,
  AvisoExitoComponent,
  EquipmentImagePlaceholderComponent,
  MostrarerrorComponent,
  PantallaCargaComponent,
} from '@shared/ui';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-carrito',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MostrarerrorComponent,
    Aviso,
    AvisoExitoComponent,
    PantallaCargaComponent,
    CalendarioComponent,
    EquipmentImagePlaceholderComponent,
  ],
  templateUrl: './carrito.component.html',
  styleUrl: './carrito.component.css',
})
export class CarritoComponent {
  public step: number = 1;
  public errorSolicitudVisible: WritableSignal<boolean> = signal(false);
  public mensajeError: string = 'Datos insertados no validos';
  confirmacionVisible: WritableSignal<boolean> = signal(false);
  exitoVisible: WritableSignal<boolean> = signal(false);
  fechaInicio: WritableSignal<Date | null> = signal(null);
  fechaFinal: WritableSignal<Date | null> = signal(null);
  carrito: Carrito = {};
  cargando = false;
  puedeReservar: WritableSignal<boolean> = signal(true);
  motivoBloqueo: WritableSignal<string> = signal('');

  private readonly fechaActual: Date = new Date();

  readonly validacionFechas = computed(() =>
    this.cartDateValidationService.validate(
      this.fechaInicio(),
      this.fechaFinal(),
      this.fechaActual,
    ),
  );

  constructor(
    private readonly carritoService: CarritoService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly usuarioService: UsuarioService,
    private readonly prestamosApiService: PrestamosAPIService,
    private readonly cartDateValidationService: CartDateValidationService,
  ) {
    this.carrito = this.carritoService.obtenerCarrito();
    this.fechaActual.setHours(0, 0, 0, 0);
    this.route.queryParams.subscribe((params) => {
      this.step = params['step'] ? Number(params['step']) : 1;
    });

    if (!this.usuarioService.estaVacio()) {
      this.prestamosApiService.estadoReserva().subscribe({
        next: (estado) => {
          this.puedeReservar.set(estado.puedeReservar);
          this.motivoBloqueo.set(estado.motivo ?? '');
        },
        error: () => {},
      });
    }
  }

  private parseDateLocal(dateString: string): Date {
    const [year, month, day] = dateString.split('-').map(Number);

    return new Date(year, month - 1, day);
  }

  get fechaInicioStr(): string {
    const fecha = this.fechaInicio();

    return fecha ? this.toLocalISOString(fecha) : '';
  }

  set fechaInicioStr(value: string) {
    this.fechaInicio.set(value ? this.parseDateLocal(value) : null);
  }

  get fechaFinalStr(): string {
    const fecha = this.fechaFinal();

    return fecha ? this.toLocalISOString(fecha) : '';
  }

  set fechaFinalStr(value: string) {
    this.fechaFinal.set(value ? this.parseDateLocal(value) : null);
  }

  nextStep(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { step: 2 },
      queryParamsHandling: 'merge',
    });
  }

  confirmarSolicitud(): void {
    const validacion = this.validacionFechas();

    if (!validacion.isValid) {
      this.mensajeError =
        validacion.message ??
        'Selecciona una fecha de inicio y una fecha final';
      this.errorSolicitudVisible.set(true);

      return;
    }

    if (this.usuarioService.estaVacio()) {
      this.router.navigate(['/Iniciar-Sesion']);

      return;
    }

    this.cambiarFechaInicio(this.fechaInicioStr);
    this.cambiarFechaFinal(this.fechaFinalStr);

    const monto = this.carritoService.calcularPrecioTotal();

    if (monto >= 1000) {
      this.router.navigate(['/Formulario']);

      return;
    }

    this.confirmacionVisible.set(true);
  }

  realizarPrestamo(): void {
    this.cargando = true;
    const carnet = this.usuarioService.obtenerUsuario().carnet!;

    this.prestamosApiService
      .crearPrestamo(this.carrito, carnet, null)
      .pipe(finalize(() => (this.cargando = false)))
      .subscribe({
        next: () => {
          this.carritoService.vaciarCarrito();
          this.exitoVisible.set(true);
        },
        error: (error) => {
          const errorMsg = extractErrorMessage(error, 'Error desconocido');
          this.mensajeError = errorMsg;
          this.errorSolicitudVisible.set(true);
        },
      });
  }

  redirigirHome(): void {
    this.router.navigate(['/home']);
  }

  carritoEstaVacio(): boolean {
    return Object.keys(this.carrito).length === 0;
  }

  botonDeshabilitado(): boolean {
    if (this.carritoEstaVacio()) return true;
    if (!this.puedeReservar()) return true;

    return !this.validacionFechas().isValid;
  }

  generarCantidadesMax(cantidad: number): number[] {
    return Array.from({ length: cantidad }, (_, i) => i + 1);
  }

  tieneImagenProducto(imagen: string | null | undefined): boolean {
    return !!imagen?.trim();
  }

  cambiarCantidad(key: string, n: number): void {
    this.carritoService.editarCantidad(Number(key), Number(n));
    this.carrito = { ...this.carritoService.obtenerCarrito() };
  }

  cambiarFechaInicio(fecha: string): void {
    this.fechaInicio.set(fecha ? this.parseDateLocal(fecha) : null);

    Object.values(this.carrito).forEach((item) => {
      item.fecha_inicio = fecha || null;
    });
  }

  cambiarFechaFinal(fecha: string): void {
    this.fechaFinal.set(fecha ? this.parseDateLocal(fecha) : null);

    Object.values(this.carrito).forEach((item) => {
      item.fecha_final = fecha || null;
    });
  }

  private toLocalISOString(date: Date): string {
    const pad = (n: number) => String(n).padStart(2, '0');

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
  }
}
