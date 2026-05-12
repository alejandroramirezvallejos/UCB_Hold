import { signal, WritableSignal } from "@angular/core";
import { PrestamoDto } from "../../../../models/admin/Prestamos";
import { PrestamoAgrupados } from "../../../../models/PrestamoAgrupados";
import { PrestamosAPIService } from "../../../../services/APIS/prestamo/prestamos-api.service";
import { UsuarioService } from "../../../../services/usuario/usuario.service";
export abstract class HistorialBase {
  datos = new Map<number, PrestamoAgrupados>;
  itemSeleccionado: PrestamoDto | null = null;
  prestamosVista: PrestamoDto[] = [];
  abrirVistaPrestamos: boolean = false;
   error: WritableSignal<boolean> = signal(false);
   mensajeerror: string=  "";
   exito: WritableSignal<boolean> = signal(false);
   mensajeexito: string= "";
  protected abstract estado: string;
  constructor(protected prestamoApi: PrestamosAPIService, protected usuario: UsuarioService) { };
  cargarDatos() {
    if (this.usuario.vacio() == false) {
      this.prestamoApi.obtenerPrestamosPorUsuario(this.usuario.obtenerDatosUsuario().id!, this.estado).subscribe({
        next: (data) => {
          this.agruparPrestamos(data);
        },
        error: (error) => {
          this.mensajeerror = "Error al cargar los prestamos, intente mas tarde";
          console.error('Error Historial:', error.error?.mensaje || error.message);
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
