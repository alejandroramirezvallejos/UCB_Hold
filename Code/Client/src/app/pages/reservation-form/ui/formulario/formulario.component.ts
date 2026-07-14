import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
  Component,
  ElementRef,
  OnInit,
  Renderer2,
  ViewChild,
  WritableSignal,
  signal,
} from '@angular/core';
import { Router } from '@angular/router';
import { Carrito } from '@entities/cart';
import { PrestamosAPIService } from '@entities/loan';
import { UsuarioService } from '@entities/user';
import { CarritoService } from '@features/cart';
import { FirmaComponent } from '@features/signature';
import { extractErrorMessage } from '@shared/lib/error';
import { escapeHtmlValue } from '@shared/lib/html';
import {
  Aviso,
  AvisoExitoComponent,
  MostrarerrorComponent,
  PantallaCargaComponent,
} from '@shared/ui';
import { finalize } from 'rxjs';

const MILLISECONDS_PER_DAY = 86_400_000;
const CONFLICT_STATUS = 409;
const UNPROCESSABLE_ENTITY_STATUS = 422;
const SERVER_ERROR_STATUS = 500;

@Component({
  selector: 'app-formulario',
  standalone: true,
  imports: [
    FirmaComponent,
    CommonModule,
    MostrarerrorComponent,
    PantallaCargaComponent,
    Aviso,
    AvisoExitoComponent,
  ],
  templateUrl: './formulario.component.html',
  styleUrl: './formulario.component.css',
})
export class FormularioComponent implements OnInit {
  @ViewChild('contratoContainer', { static: false })
  contratoContainer!: ElementRef;

  contenidoHtml: string = '';
  clickfirma: WritableSignal<boolean> = signal(false);
  firma: string = '';
  error: WritableSignal<boolean> = signal(false);
  mensajeerror: string = 'Error desconocido intente mas tarde';
  cargando: boolean = false;
  aviso: WritableSignal<boolean> = signal(false);
  mensajeaviso: string =
    'Aviso desconocido , si ve esto es un error , avise al soporte si puede o intente mas tarde';
  avisoexito: WritableSignal<boolean> = signal(false);
  mensajeexito: string =
    'Aviso de exito desconocido , si ve esto es un error , avise al soporte si puede o intente mas tarde';

  constructor(
    private readonly http: HttpClient,
    private readonly renderer: Renderer2,
    private readonly carrito: CarritoService,
    private readonly router: Router,
    private readonly usuario: UsuarioService,
    private readonly mandarprestamo: PrestamosAPIService,
  ) {}

  ngOnInit(): void {
    const fechaInicioReserva = this.carrito.obtenerFechaInicio();
    const fechaFinalReserva = this.carrito.obtenerFechaFinal();

    if (!fechaInicioReserva || !fechaFinalReserva) {
      this.mensajeerror =
        'Seleccione fechas de préstamo antes de generar el contrato.';
      this.error.set(true);
      return;
    }

    const fechaInicio = new Date(fechaInicioReserva);
    const fechaFinal = new Date(fechaFinalReserva);
    const diffDias = Math.ceil(
      (fechaFinal.getTime() - fechaInicio.getTime()) / MILLISECONDS_PER_DAY,
    );
    this.http.get('assets/contrato.html', { responseType: 'text' }).subscribe({
      next: (data: string) => {
        const processedTemplate = this.reemplazarMarcadores(data, {
          dia: new Date().getDate().toString(),
          mesliteral: new Intl.DateTimeFormat('es-ES', {
            month: 'long',
          }).format(new Date()),
          año: new Date().getFullYear().toString(),
          usuario: escapeHtmlValue(this.usuario.obtenerUsuario().nombre ?? ''),
          usuario_ci: escapeHtmlValue(
            this.usuario.obtenerUsuario().carnet ?? '',
          ),
          tablaprimera: this.primeradelobjeto(this.carrito.obtenerCarrito()),
          fechaMaxima: String(diffDias),
          precio: this.carrito.calcularPrecioTotal().toString(),
          tablasegunda: this.quintavalordebienes(this.carrito.obtenerCarrito()),
          dia_devolucion: (fechaFinal.getDate() + 1).toString(),
          mes_devolucion: (fechaFinal.getMonth() + 1).toString(),
          año_devolucion: fechaFinal.getFullYear().toString(),
          firmausuario: '',
        });
        this.contenidoHtml = processedTemplate;
      },
      error: (error) => {
        const errorMsg = extractErrorMessage(
          error,
          'Error al cargar el contrato, intente mas tarde',
        );
        this.mensajeerror = errorMsg;
        this.error.set(true);
      },
    });
  }

  private reemplazarMarcadores(
    template: string,
    valores: { [clave: string]: string },
  ): string {
    let resultado = template;
    for (const clave in valores) {
      const regex = new RegExp(`\\[\\[${clave}\\]\\]`, 'g');
      resultado = resultado.replace(regex, valores[clave]);
    }
    return resultado;
  }

  firmar() {
    this.clickfirma.set(true);
  }

  aceptar() {
    if (
      !this.carrito ||
      Object.keys(this.carrito.obtenerCarrito()).length === 0
    ) {
      this.mensajeerror =
        'El carrito está vacío. Agregue elementos antes de continuar.';
      this.error.set(true);
    } else if (!this.firma || this.firma === '') {
      this.firmar();
    } else {
      this.aviso.set(true);
      this.mensajeaviso =
        '¿Está seguro de confirmar el préstamo con los términos y condiciones establecidos en el contrato?';
    }
  }

  confirmarprestamo() {
    const contratoTexto = this.generarHTMLTexto();
    this.cargando = true;
    this.mandarprestamo
      .crearPrestamo(
        this.carrito.obtenerCarrito(),
        this.usuario.obtenerUsuario().carnet!,
        contratoTexto,
      )
      .pipe(finalize(() => (this.cargando = false)))
      .subscribe({
        next: (_response) => {
          this.mensajeexito = 'El préstamo ha sido creado exitosamente.';
          this.avisoexito.set(true);
          this.carrito.vaciarCarrito();
        },
        error: (error) => {
          this.error.set(true);
          if (error?.status === CONFLICT_STATUS) {
            this.mensajeerror =
              'Ya tienes una solicitud activa para este equipo. Espera a que finalice antes de hacer una nueva reserva.';
          } else if (error?.status === UNPROCESSABLE_ENTITY_STATUS) {
            this.mensajeerror =
              'Los datos de tu solicitud no son válidos. Revisa las fechas e intenta nuevamente.';
          } else if (error?.status === SERVER_ERROR_STATUS) {
            this.mensajeerror =
              'Ocurrió un error en el servidor. Por favor, inténtalo de nuevo más tarde.';
          } else {
            this.mensajeerror =
              'No se pudo procesar tu solicitud. Verifica tu conexión e inténtalo nuevamente.';
          }
        },
      });
  }

  irhome() {
    this.router.navigate(['/home']);
  }

  guardarfirma(signatureData: string): void {
    this.firma = signatureData;
    if (this.contratoContainer) {
      const signatureImage: HTMLElement | null =
        this.contratoContainer.nativeElement.querySelector(
          '#firmaUsuarioPlaceholder',
        );
      if (signatureImage) {
        this.renderer.setAttribute(signatureImage, 'src', this.firma);
      }
    }
  }

  private primeradelobjeto(carrito: Carrito): string {
    const items = Object.entries(carrito).filter(
      ([, item]) => typeof item === 'object' && 'nombre' in item,
    );
    return `
      ${items
        .map(
          ([key, item]) => `
        <tr>
          <td class="imt-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td class="ucb-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td>
          <strong>${escapeHtmlValue(item.nombre ?? '')}</strong>
          <p>Marca: ${escapeHtmlValue(item.marca ?? '')} </p>
          <p>Modelo: ${escapeHtmlValue(item.modelo ?? '')} </p>
          </td>
          <td class="serial-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td>${item.cantidad}</td>
        </tr>
      `,
        )
        .join('')}
    `;
  }

  private quintavalordebienes(carrito: Carrito): string {
    const items = Object.entries(carrito).filter(
      ([, item]) => typeof item === 'object' && 'nombre' in item,
    );
    return `
      ${items
        .map(
          ([key, item]) => `
        <tr>
          <td class="imt-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td class="ucb-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td>
          <strong>${escapeHtmlValue(item.nombre ?? '')}</strong>
          <p>Marca: ${escapeHtmlValue(item.marca ?? '')} </p>
          <p>Modelo: ${escapeHtmlValue(item.modelo ?? '')} </p>
          </td>
          <td class="serial-code" data-grupo-id="${escapeHtmlValue(key)}">Por definirse</td>
          <td>${item.cantidad}</td>
          <td>${item.precio}</td>
          <td>${item.precio * item.cantidad}</td>
        </tr>
      `,
        )
        .join('')}
    `;
  }

  generarHTMLTexto(): string {
    const contratoElement = this.contratoContainer.nativeElement;
    return contratoElement.outerHTML;
  }
}
