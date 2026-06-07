import { Component, EventEmitter, Input, Output, signal, WritableSignal } from '@angular/core';
import { PrestamoDto } from '../../models/admin/Prestamos';
@Component({
  selector: 'app-vista-prestamos',
  imports: [],
  templateUrl: './vista-prestamos.component.html',
  styleUrl: './vista-prestamos.component.css'
})
export class VistaPrestamosComponent {
  @Input() prestamos: PrestamoDto[] = [];
  @Output() cerrar : EventEmitter<void> = new EventEmitter<void>();
  cerrarVista(){
    this.cerrar.emit();
  }
  // Datos de prueba antiguos guardaron literalmente "string" como observación; lo tratamos como vacío.
  observacionTexto(): string {
    const obs = this.prestamos[0]?.Observacion?.trim();
    return obs && obs.toLowerCase() !== 'string' ? obs : 'Sin observación';
  }
}
