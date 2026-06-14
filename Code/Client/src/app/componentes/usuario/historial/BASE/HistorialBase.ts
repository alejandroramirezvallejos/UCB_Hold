import { Directive, Input, signal, WritableSignal } from "@angular/core";
import { PrestamoDto } from "../../../../models/admin/Prestamos";
import { PrestamoAgrupados } from "../../../../models/PrestamoAgrupados";
import { PrestamosAPIService } from "../../../../services/APIS/prestamo/prestamos-api.service";
import { UsuarioService } from "../../../../services/usuario/usuario.service";
import { extractErrorMessage } from "../../../../utils/error-handler";
@Directive()
export abstract class HistorialBase {
  @Input() filtroTexto: string = '';
  @Input() fechaDesde: string = '';
  @Input() fechaHasta: string = '';
  datos = new Map<number, PrestamoAgrupados>;
  itemSeleccionado: PrestamoDto | null = null;
  prestamosVista: PrestamoDto[] = [];
  abrirVistaPrestamos: boolean = false;
   error: WritableSignal<boolean> = signal(false);
   mensajeerror: string=  "";
   exito: WritableSignal<boolean> = signal(false);
   mensajeexito: string= "";
  protected abstract estado: string;
  // Mantiene el orden de inserción del Map (backend ya ordena por FechaSolicitud desc → más recientes arriba),
  // en vez del orden ascendente por key que aplica el pipe keyvalue por defecto.
  keepOrder = (_a: unknown, _b: unknown): number => 0;
  constructor(protected prestamoApi: PrestamosAPIService, protected usuario: UsuarioService) { };
  get datosFiltrados(): Map<number, PrestamoAgrupados> {
    const texto = this.filtroTexto.trim().toLowerCase();
    const desde = this.fechaDesde ? new Date(this.fechaDesde) : null;
    const hasta = this.fechaHasta ? new Date(this.fechaHasta + 'T23:59:59') : null;
    if (!texto && !desde && !hasta) return this.datos;
    const filtrado = new Map<number, PrestamoAgrupados>();
    for (const [id, grupo] of this.datos) {
      if (texto) {
        const matchId = id.toString().includes(texto);
        const matchNombre = (grupo.datosgrupo.NombreGrupoEquipo ?? '').toLowerCase().includes(texto);
        if (!matchId && !matchNombre) continue;
      }
      if (desde || hasta) {
        const fecha = grupo.datosgrupo.FechaSolicitud ? new Date(grupo.datosgrupo.FechaSolicitud) : null;
        if (!fecha) continue;
        if (desde && fecha < desde) continue;
        if (hasta && fecha > hasta) continue;
      }
      filtrado.set(id, grupo);
    }
    return filtrado;
  }
  cargarDatos() {
    if (this.usuario.vacio() == false) {
      this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.obtenerDatosUsuario().id!, this.estado).subscribe({
        next: (data) => {
          this.agruparPrestamos(data);
        },
        error: (error) => {
          const errorMsg = extractErrorMessage(error, 'Error al cargar los prestamos, intente mas tarde');
          this.mensajeerror = errorMsg;
          console.error('Error Historial:', errorMsg);
          this.error.set(true);
        }
      });
    }
  }
  agruparPrestamos(datos: PrestamoDto[]) {
    // Nueva referencia → Angular change detection dispara
    const nuevo = new Map<number, PrestamoAgrupados>();
    for (let prestamo of datos) {
      if (nuevo.has(prestamo.Id!)) {
        nuevo.get(prestamo.Id)!.insertarEquipo(prestamo);
      } else {
        nuevo.set(prestamo.Id!, new PrestamoAgrupados([prestamo]));
      }
    }
    this.datos = nuevo;
  }
  AbrirVista(item: PrestamoDto[]) {
    this.prestamosVista = item;
    this.abrirVistaPrestamos = true;
  }
  cerrarVista() {
    this.abrirVistaPrestamos = false;
    this.prestamosVista = [];
  }
}
